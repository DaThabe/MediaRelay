using MediaRelay.Extensions;
using MediaRelay.Persistent.Url.Messages;
using MediaRelay.Url;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace MediaRelay.Persistent.Url;

internal sealed partial class UrlPersistentQueueFactory(
        ILoggerFactory loggerFactory,
        IUrlQueueStore store
    ) : IUrlPersistentQueueFactory, IDisposable
{
    private bool _disposed;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private InternalQueue? _instance;



    public async ValueTask<IUrlPersistentQueue> GetOrCreateAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Lock
        if (_instance is not null) return _instance;
        using var _ = await _lock.WaitScopeAsync(cancellationToken);

        // Create
        var messages = await store.LoadAsync(cancellationToken);
        var logger = loggerFactory.CreateLogger<InternalQueue>();

        var faileds = messages.OfType<FailedMessage>().OrderBy(x => x.CreatedAt);
        var pendings = messages.OfType<PendingMessage>().OrderBy(x => x.CreatedAt);
        var deads = messages.OfType<DeadMessage>().OrderBy(x => x.CreatedAt);

        Channel<Message> pendingChannel = Channel.CreateUnbounded<Message>();
        Channel<Message> deadChannel = Channel.CreateUnbounded<Message>();

        foreach (var i in faileds) await pendingChannel.Writer.WriteAsync(i, cancellationToken);
        foreach (var i in pendings) await pendingChannel.Writer.WriteAsync(i, cancellationToken);
        foreach (var i in deads) await deadChannel.Writer.WriteAsync(i, cancellationToken);


        return _instance = new InternalQueue(store, logger)
        {
            Pendings = pendingChannel,
            Deads = deadChannel
        };
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _lock.Dispose();
    }
}

internal sealed partial class UrlPersistentQueueFactory
{
    private sealed class InternalQueue(IUrlQueueStore store, ILogger<InternalQueue> logger) : IUrlPersistentQueue
    {
        public required Channel<Message> Pendings { get; init; }
        public required Channel<Message> Deads { get; init; }
        public int Count => Pendings.Reader.Count;



        public async ValueTask WriteAsync(Uri url, CancellationToken cancellationToken)
        {
            logger.LogInformation("正在写入队列");
            await Pendings.Writer.WriteAsync(PendingMessage.Create(url), cancellationToken);
            logger.LogTrace("已写入队列");

            var messages = await GetAllMessageAsync(cancellationToken);
            await store.SaveAsync(messages, cancellationToken);
        }

        public async ValueTask<Uri> ReadWaitAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("等待读取消息");
            if (!await Pendings.Reader.WaitToReadAsync(cancellationToken))
                throw new InvalidOperationException("队列已关闭");

            var message = await Pendings.Reader.ReadAsync(cancellationToken);
            using var _ = logger.BeginScope("Message", message);
            logger.LogInformation("已读取消息");

            var messages = await GetAllMessageAsync(cancellationToken);
            await store.SaveAsync(messages, cancellationToken);

            return message.Content;
        }
        public async ValueTask<Uri> PeepWaitAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("等待查看消息");
            if (!await Pendings.Reader.WaitToReadAsync(cancellationToken))
                throw new InvalidOperationException("队列已关闭");

            if (!Pendings.Reader.TryPeek(out var message))
                throw new InvalidOperationException("队列数据异常");

            using var _ = logger.BeginScope("Message", message);
            logger.LogInformation("查看消息");

            return message.Content;
        }


        // 获取所有消息
        async ValueTask<Message[]> GetAllMessageAsync(CancellationToken cancellationToken)
        {
            var messages = new List<Message>();
            messages.AddRange(await GetChannelValuesAsync(Pendings, cancellationToken));
            messages.AddRange(await GetChannelValuesAsync(Deads, cancellationToken));

            return [.. messages.OrderBy(x => x.CreatedAt)];
        }
        // 获取通道的所有元素
        static async ValueTask<T[]> GetChannelValuesAsync<T>(Channel<T> channel, CancellationToken cancellationToken = default)
        {
            var values = new List<T>();
            while (channel.Reader.TryRead(out var item)) values.Add(item);

            foreach (var i in values) await channel.Writer.WriteAsync(i, cancellationToken);
            return [.. values];
        }

       
    }
}