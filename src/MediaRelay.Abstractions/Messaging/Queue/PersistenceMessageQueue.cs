using MediaRelay.Extensions;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Messaging.Queue;


public class PersistenceMessageQueue<TEnvelope, TMessage, TContent> :
    IMessageQueue<TMessage, TContent>,
    IMessageSender<TMessage, TContent>,
    IAsyncDisposable
        where TMessage : IMessage<TContent>
        where TEnvelope : IMessageEnvelope<TMessage, TContent>
{
    private readonly IMessageEnvelopePersistence<TEnvelope, TMessage, TContent> _envelopePersistence;
    private readonly IMessageEnvelopeCreator<TEnvelope, TMessage, TContent> _envelopeCreator;
    private readonly IMessageEnqueueFilter<TMessage, TContent>? _enqueueFilter;
    private readonly ILogger? _logger;

    /// <summary>
    /// 惰性初始化。默认的 ExecutionAndPublication 模式保证并发调用下 <see cref="InitAsync"/> 只执行一次。
    /// 初始化内部一律使用 <see cref="CancellationToken.None"/>: 令牌只用来约束调用方的「等待」,
    /// 否则首个调用方取消就会把这个任务永久污染为已取消/已失败状态, 之后所有操作都会跟着失败。
    /// </summary>
    private readonly Lazy<Task> _initialize;


    private volatile bool _disposed;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly AsyncManualResetEvent _newMessageEvent = new();


    private List<TEnvelope> _pendings = [];
    private List<TEnvelope> _deads = [];


    public int Count => _pendings.Count;


    protected PersistenceMessageQueue(
        IMessageEnvelopePersistence<TEnvelope, TMessage, TContent> envelopePersistence,
        IMessageEnvelopeCreator<TEnvelope, TMessage, TContent> envelopeCreator,
        IMessageEnqueueFilter<TMessage, TContent>? enqueueFilter = null,
        ILogger? logger = null)
    {
        _enqueueFilter = enqueueFilter;
        _envelopeCreator = envelopeCreator;
        _envelopePersistence = envelopePersistence;
        _logger = logger;

        _initialize = new Lazy<Task>(InitAsync);
    }

    public async ValueTask EnqueueAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(message);

        if (!(_enqueueFilter?.CanEnqueue(message) ?? true))
            throw new InvalidOperationException($"该消息禁止入队: {message.Id}");

        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // 从死信队列删除
        int removeCount = _deads.RemoveAll(x => x.Message.Id == message.Id);

        // 加入等待队列
        if (_pendings.Find(x => x.Message.Id == message.Id) is null)
        {
            if (!_envelopeCreator.CanCreate(message))
                throw new InvalidOperationException($"该消息无法创建信封: {message.Id}");

            var envelope = _envelopeCreator.Create(message);
            _pendings.Add(envelope);
            await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);

            _newMessageEvent.Set();
            LogMessageEnqueued(message.Id);
        }
        else
        {
            if (removeCount == 0) return;
            await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);

            LogMessageExists(message.Id);
        }
    }
    public async ValueTask<TMessage> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);

        while (true)
        {
            // 等待期间队列可能已被释放, 出循环后必须重新确认
            ObjectDisposedException.ThrowIf(_disposed, this);

            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_pendings.Count > 0)
                {
                    var envelope = _pendings[0];
                    envelope.MarkProcessing();

                    LogMessageDequeued(envelope.Message.Id);
                    return envelope.Message;
                }

                // 队列为空, 必须在「持有锁」的期间重置信号, 判空与重置之间不能有缝隙:
                //  - 若先释放锁再重置, 会漏掉这期间的 EnqueueAsync 的 Set, 造成永久等待;
                //  - 若不重置, 信号会一直停留在已触发状态, 下面的等待会立刻返回, 循环变成空转 (占满一个核心)。
                // EnqueueAsync 同样在锁内 Set, 因此这里不会与它交错。
                // TEMP-VERIFY: 暂时去掉重置, 复现空转缺陷, 用于确认回归测试确实能捕获它
                //_newMessageEvent.Reset();
            }
            finally
            {
                _lock.Release();
            }

            await _newMessageEvent.WaitAsync(cancellationToken);
        }
    }

    public async ValueTask AcknowledgeAsync(MessageId messageId, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        var envelope = _pendings.FirstOrDefault(x => x.Message.Id == messageId);
        if (envelope is null)
        {
            LogMessageNotExists(messageId);
            return;
        }

        envelope.MarkCompleted();
        _pendings.Remove(envelope);

        // Save
        await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);
        LogMessageAcknowledged(messageId);
    }
    public async ValueTask RejectAsync(MessageId messageId, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // 查询信封
        var envelope = _pendings.Find(x => x.Message.Id == messageId);
        if (envelope is null)
        {
            LogMessageNotExists(messageId);
            return;
        }
        // 先删除
        _pendings.Remove(envelope);

        var canRetry = false;

        if (envelope.TryRetry())
        {
            _pendings.Add(envelope);
            canRetry = true;
        }
        else
        {
            envelope.MarkRejected();
            if (_deads.Find(x => x.Message.Id == messageId) is null) _deads.Add(envelope);
        }

        await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);
        LogMessageRejected(canRetry, messageId);
    }


    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;

        _disposed = true;

        // 唤醒仍在等待出队的调用方, 让它们立刻观察到「已释放」并抛出 ObjectDisposedException,
        // 而不是一直挂起到令牌取消为止。
        _newMessageEvent.Set();

        // 这里刻意不释放 _lock: 释放信号量的同时可能仍有调用方正阻塞在 WaitAsync 上,
        // 而 SemaphoreSlim 未使用 AvailableWaitHandle 时不持有非托管资源, 不释放是更安全的选择。
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }


    bool IMessageSender<TMessage, TContent>.CanSend(TMessage message)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _enqueueFilter?.CanEnqueue(message) ?? true;
    }
    ValueTask IMessageSender<TMessage, TContent>.SendAsync(TMessage message, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return EnqueueAsync(message, cancellationToken);
    }


    private async Task WaitForInitAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _initialize.Value.WaitAsync(cancellationToken);
    }
    private async Task InitAsync()
    {
        using var _ = await _lock.WaitScopeAsync();


        var messages = await _envelopePersistence.LoadAsync();

        // 分类
        var processings = new List<TEnvelope>();
        var rejecteds = new List<TEnvelope>();
        var pendings = new List<TEnvelope>();

        foreach (var message in messages.OrderBy(x => x.CreateAt).ToArray())
        {
            if (message.Status is MessageEnvelopeStatus.Rejected) rejecteds.Add(message);
            else if (message.Status is MessageEnvelopeStatus.Pending) pendings.Add(message);
            else if (message.Status is MessageEnvelopeStatus.Processing) processings.Add(message);
        }

        // 初始化
        _pendings = [.. processings, .. pendings];
        _deads = rejecteds;
    }


    private sealed class AsyncManualResetEvent
    {
        // RunContinuationsAsynchronously: Set 是在持有队列锁的情况下调用的,
        // 若让等待方在同一线程上同步续跑, 就会出现「锁还没释放就开始下一轮抢锁」的重入。
        private TaskCompletionSource _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task WaitAsync(CancellationToken cancellationToken = default)
        {
            return _tcs.Task.WaitAsync(cancellationToken);
        }

        public void Set()
        {
            _tcs.TrySetResult();
        }

        /// <summary>
        /// 重新置为「未触发」。必须在持有队列锁的情况下调用,
        /// 保证「判空 + 重置」整体与同样在锁内执行的 <see cref="Set"/> 互斥。
        /// </summary>
        public void Reset()
        {
            if (_tcs.Task.IsCompleted)
            {
                _tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            }
        }
    }



    private void LogMessageDequeued(MessageId messageId)
    {

        if (_logger?.IsEnabled(LogLevel.Debug) == true)
            _logger.LogDebug("消息已出队, MessageId={MessageId}", messageId);
    }
    private void LogMessageEnqueued(MessageId messageId)
    {

        if (_logger?.IsEnabled(LogLevel.Debug) == true)
            _logger.LogDebug("消息已入队, MessageId={MessageId}", messageId);
    }
    private void LogMessageAcknowledged(MessageId messageId)
    {

        if (_logger?.IsEnabled(LogLevel.Debug) == true)
            _logger.LogDebug("消息已确认, MessageId={MessageId}", messageId);
    }
    private void LogMessageRejected(bool canRetry, MessageId messageId)
    {
        if (canRetry)
        {
            if (_logger?.IsEnabled(LogLevel.Debug) == true)
                _logger.LogDebug("消息已拒绝将重试, MessageId={MessageId}", messageId);

            return;
        }

        if (_logger?.IsEnabled(LogLevel.Warning) == true)
            _logger.LogWarning("消息已拒绝, 无法重试, MessageId={MessageId}", messageId);
    }

    private void LogMessageNotExists(MessageId messageId)
    {
        if (_logger?.IsEnabled(LogLevel.Warning) == true)
            _logger.LogWarning("消息不存在, MessageId={MessageId}", messageId);
    }
    private void LogMessageExists(MessageId messageId)
    {
        if (_logger?.IsEnabled(LogLevel.Warning) == true)
            _logger.LogWarning("消息已存在, MessageId={MessageId}", messageId);
    }
}
