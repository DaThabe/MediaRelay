using MediaRelay.Extensions;

namespace MediaRelay.Messaging;

public abstract class PersistenceMessageQueue<TMessage, TContent> : IMessageQueue<TMessage, TContent>, IAsyncDisposable
    where TMessage : IMessage<TContent>
{
    private readonly CancellationTokenSource _innerTaskCts = new();
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly TaskCompletionSource _loadTcs = new();
    private readonly AsyncManualResetEvent _newMessageEvent = new();


    private List<IMessageEnvelope<TMessage, TContent>> _pendings = [];
    private List<IMessageEnvelope<TMessage, TContent>> _deads = [];


    protected PersistenceMessageQueue()
    {
        Task.Run(() => InitAsync(_innerTaskCts.Token), _innerTaskCts.Token);
    }

    public async ValueTask SendAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        var envelope = CreateEnvelope(message);

        _pendings.Add(envelope);
        // Save
        await SaveAsync([.. _pendings, .. _deads], cancellationToken);

        _newMessageEvent.Set();
    }
    public ValueTask<TMessage> PeekAsync(CancellationToken cancellationToken = default)
    {
        return WaitGetMessageAsync(
           () => _pendings.Count > 0,
           _ => ValueTask.FromResult(_pendings[^1].Message),
           cancellationToken);
    }
    public ValueTask<TMessage> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        return WaitGetMessageAsync(
            () => _pendings.Count > 0,
            async ct =>
            {
                var item = _pendings[^1];
                _pendings.Remove(item);

                await SaveAsync([.. _pendings, .. _deads], ct);
                return item.Message;
            },
            cancellationToken);
    }

    public async ValueTask AcknowledgeAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        _pendings.RemoveAll(x => x.Message.Id == message.Id);

        // Save
        await SaveAsync([.. _pendings, .. _deads], cancellationToken);
    }
    public async ValueTask RejectAsync(TMessage message, bool requeue = true, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        var envelope = CreateEnvelope(message);

        if (requeue) _pendings.Add(envelope);
        else _deads.Add(envelope);

        // Save
        await SaveAsync([.. _pendings, .. _deads], cancellationToken);
    }


    public async ValueTask DisposeAsync()
    {
        await _innerTaskCts.CancelAsync();
        _innerTaskCts.Dispose();
    }


    protected abstract ValueTask<IEnumerable<IMessageEnvelope<TMessage, TContent>>> LoadAsync(CancellationToken cancellationToken = default);
    protected abstract ValueTask SaveAsync(IEnumerable<IMessageEnvelope<TMessage, TContent>> data, CancellationToken cancellationToken = default);
    protected abstract IMessageEnvelope<TMessage, TContent> CreateEnvelope(TMessage message);


    private async ValueTask<TMessage> WaitGetMessageAsync(Func<bool> canGet, Func<CancellationToken, ValueTask<TMessage>> getMessageAsync, CancellationToken cancellationToken = default)
    {
        await WaitForInitAsync(cancellationToken);

        while (!cancellationToken.IsCancellationRequested && !_innerTaskCts.IsCancellationRequested)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                if (canGet())
                    return await getMessageAsync(cancellationToken);
            }
            finally
            {
                _lock.Release();
            }

            await _newMessageEvent.WaitAsync(cancellationToken);
        }

        throw new OperationCanceledException(cancellationToken);
    }
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
            var faileds = new List<IMessageEnvelope<TMessage, TContent>>();
            var pendings = new List<IMessageEnvelope<TMessage, TContent>>();
            var deads = new List<IMessageEnvelope<TMessage, TContent>>();

            foreach (var message in messages.OrderBy(x => x.CreateAt).ToArray())
            {
                if (message.Status is MessageStatus.Failed) faileds.Add(message);
                else if (message.Status is MessageStatus.Pending) pendings.Add(message);
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