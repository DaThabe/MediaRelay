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


    private bool _disposed;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private Task? _initTask;
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
    }

    public async ValueTask EnqueueAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(message);

        if (!(_enqueueFilter?.CanEnqueue(message) ?? true))
            throw new InvalidOperationException($"该消息禁止入队: {message.Id}");

        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);
        using var __ = _logger?.BeginScope("MessageId", message.Id);

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
            _logger?.LogDebug("消息已入队");
        }
        else
        {
            _logger?.LogWarning("消息已存在");
            if (removeCount == 0) return;

            await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);
        }
    }
    public async ValueTask<TMessage> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_pendings.Count > 0)
                {
                    var envelope = _pendings[0];
                    envelope.MarkProcessing();

                    using var _ = _logger?.BeginScope("MessageId", envelope.Message.Id);
                    _logger?.LogDebug("消息已出队");


                    return envelope.Message;
                }
            }
            finally
            {
                _lock.Release();
            }

            await _newMessageEvent.WaitAsync(cancellationToken);
        }

        throw new OperationCanceledException(cancellationToken);
    }

    public async ValueTask AcknowledgeAsync(MessageId messageId, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);
        using var __ = _logger?.BeginScope("MessageId", messageId);

        var envelope = _pendings.FirstOrDefault(x => x.Message.Id == messageId);
        if (envelope is null)
        {
            _logger?.LogWarning("消息不存在");
            return;
        }

        envelope.MarkCompleted();
        _pendings.Remove(envelope);

        // Save
        await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);

        _logger?.LogDebug("消息已确认");
    }
    public async ValueTask RejectAsync(MessageId messageId, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);
        using var __ = _logger?.BeginScope("MessageId", messageId);

        // 查询信封
        var envelope = _pendings.Find(x => x.Message.Id == messageId);
        if (envelope is null)
        {
            _logger?.LogWarning("消息已不存在");
            return;
        }
        // 先删除
        _pendings.Remove(envelope);

        var hasRetry = false;

        if (envelope.TryRetry())
        {
            _pendings.Add(envelope);
            hasRetry = true;
        }
        else
        {
            envelope.MarkRejected();
            if (_deads.Find(x => x.Message.Id == messageId) is null) _deads.Add(envelope);
        }

        await _envelopePersistence.SaveAsync([.. _pendings, .. _deads], cancellationToken);

        if (hasRetry) _logger?.LogDebug("消息将重试");
        else _logger?.LogWarning("消息无法重试, 已拒绝");
    }


    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {

        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "持久化消息队列释放失败");
        }

        GC.SuppressFinalize(this);
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

        _initTask ??= InitAsync(cancellationToken);
        await _initTask.WaitAsync(cancellationToken);
    }
    private async Task InitAsync(CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);


        var messages = await _envelopePersistence.LoadAsync(cancellationToken);

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
        private TaskCompletionSource _tcs = new();

        public Task WaitAsync(CancellationToken cancellationToken = default)
        {
            return _tcs.Task.WaitAsync(cancellationToken);
        }

        public void Set()
        {
            _tcs.TrySetResult();
        }

        public void Reset()
        {
            if (_tcs.Task.IsCompleted)
            {
                _tcs = new TaskCompletionSource();
            }
        }
    }
}