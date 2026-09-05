using Microsoft.Extensions.Logging;

namespace MediaRelay.Publish;


internal sealed class PublishOrchestrator(
    IEnumerable<IPublishExecutor> executors,
    ILogger<PublishOrchestrator> logger
    ) : IPublishOrchestrator
{
    private readonly IPublishExecutor[] _executors = [.. executors];

    public async ValueTask PublishAsync(PublishContent content, CancellationToken cancellationToken = default)
    {
        if (_executors.Length == 0) return;

        var tasks = new List<Task>();

        foreach (var executor in _executors)
        {
            var task = executor.ExecuteAsync(content, cancellationToken).AsTask();
            tasks.Add(task);
        }

        logger.LogInformation("正在推送");

        await Task.WhenAll(tasks);

        logger.LogInformation("推送完成");
    }
}
