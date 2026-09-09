using MediaRelay.Extensions;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Messaging.Queue;


public abstract class PersistenceMessageQueue<TEnvelope, TMessage, TContent> : IMessageQueue<TMessage, TContent>, IAsyncDisposable
    where TMessage : IMessage<TContent>
    where TEnvelope : IMessageEnvelope<TMessage, TContent>
{
    private readonly ILogger? _logger;
    private readonly CancellationTokenSource _innerTaskCts = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly TaskCompletionSource _loadTcs = new();
    private readonly AsyncManualResetEvent _newMessageEvent = new();

    private List<TEnvelope> _pendings = [];
    private List<TEnvelope> _deads = [];


    protected PersistenceMessageQueue(ILogger? logger = null)
    {
        _logger = logger;
        Task.Run(() => InitAsync(_innerTaskCts.Token), _innerTaskCts.Token);
    }


    public async ValueTask EnqueueAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // 从死信队列删除
        _deads.RemoveAll(x => x.Message.Id == message.Id);

        if (_pendings.Find(x => x.Message.Id == message.Id) is not null)
        {
            LogEnqueueExists(message);
            return;
        }

        var envelope = CreateEnvelope(message);
        _pendings.Add(envelope);

        // Save
        await SaveAsync([.. _pendings, .. _deads], cancellationToken);
        LogEnqueued(envelope);

        _newMessageEvent.Set();
    }
    public async ValueTask<TMessage> DequeueAsync(CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested && !_innerTaskCts.IsCancellationRequested)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (_pendings.Count > 0)
                {
                    var envelope = _pendings[^1];
                    envelope.MarkProcessing();

                    LogDequeued(envelope);
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

    public async ValueTask AcknowledgeAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        var envelope = _pendings.FirstOrDefault(x => x.Message.Id == message.Id);
        if (envelope is null)
        {
            LogEnqueueNotExists(message);
            return;
        }

        envelope.MarkCompleted();
        _pendings.Remove(envelope);

        // Save
        await SaveAsync([.. _pendings, .. _deads], cancellationToken);
        LogAcknowledge(envelope);
    }
    public async ValueTask RejectAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // 查询信封
        var envelope = _pendings.Find(x => x.Message.Id == message.Id);
        if (envelope is null)
        {
            _logger?.LogWarning("消息已不存在");
            return;
        }
        // 先删除
        _pendings.Remove(envelope);


        try
        {
            envelope.Retry();

            _pendings.Add(envelope);
            await SaveAsync([.. _pendings, .. _deads], cancellationToken);

            _logger?.LogInformation("消息已重试");
        }
        catch (Exception ex)
        {
            envelope.MarkRejected();
            if (_deads.Find(x => x.Message.Id == message.Id) is null) _deads.Add(envelope);

            await SaveAsync([.. _pendings, .. _deads], cancellationToken);
            _logger?.LogInformation(ex, "消息已拒绝");
        }
    }


    public async ValueTask DisposeAsync()
    {
        await _innerTaskCts.CancelAsync();
        _innerTaskCts.Dispose();

        GC.SuppressFinalize(this);
    }


    protected abstract ValueTask<IEnumerable<TEnvelope>> LoadAsync(CancellationToken cancellationToken = default);
    protected abstract ValueTask SaveAsync(IEnumerable<TEnvelope> envelopes, CancellationToken cancellationToken = default);
    protected abstract TEnvelope CreateEnvelope(TMessage message);


    private async Task WaitForInitAsync(CancellationToken cancellationToken = default)
    {
        await _loadTcs.Task.WaitAsync(cancellationToken);
    }
    private async Task InitAsync(CancellationToken cancellationToken)
    {
        using var _ = await _lock.WaitScopeAsync(cancellationToken);
        try
        {
            var messages = await LoadAsync(cancellationToken);

            // 分类
            var faileds = new List<TEnvelope>();
            var pendings = new List<TEnvelope>();
            var deads = new List<TEnvelope>();

            foreach (var message in messages.OrderBy(x => x.CreateAt).ToArray())
            {
                if (message.Status is MessageEnvelopeStatus.Rejected) faileds.Add(message);
                else if (message.Status is MessageEnvelopeStatus.Pending) pendings.Add(message);
                else deads.Add(message);
            }

            // 初始化
            _pendings = [.. faileds, .. pendings];
            _deads = deads;

            _loadTcs.TrySetResult();
        }
        catch (Exception ex)
        {
            _loadTcs.TrySetException(ex);
        }
    }


    private void LogEnqueueExists(TMessage _)
    {
        if (_logger is null) return;
        _logger.LogWarning("消息已存在");
    }
    private void LogEnqueueNotExists(TMessage _)
    {
        if (_logger is null) return;
        _logger.LogWarning("消息不存在");
    }
    private void LogEnqueued(IMessageEnvelope<TMessage, TContent> envelope)
    {
        if (_logger is null) return;

        using var _ = _logger.BeginScope("Envelope", envelope);
        _logger.LogDebug("已入队");
    }
    private void LogDequeued(IMessageEnvelope<TMessage, TContent> envelope)
    {
        if (_logger is null) return;

        using var _ = _logger.BeginScope("Envelope", envelope);
        _logger.LogDebug("已出队");
    }
    private void LogAcknowledge(IMessageEnvelope<TMessage, TContent> envelope)
    {
        if (_logger is null) return;

        using var _ = _logger.BeginScope("Envelope", envelope);
        _logger.LogDebug("已确认");
    }
    private void LogReject(IMessageEnvelope<TMessage, TContent> envelope)
    {
        if (_logger is null) return;

        using var _ = _logger.BeginScope("Envelope", envelope);
        _logger.LogDebug("已拒绝");
    }



    protected sealed class AsyncManualResetEvent
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