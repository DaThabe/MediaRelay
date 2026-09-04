using MediaRelay.Source;
using Microsoft.Extensions.Logging;

namespace MediaRelay.Publish;


internal sealed partial class PublishOrchestrator(
    IEnumerable<IPublishExecutor> executors,
    ILogger<PublishOrchestrator> logger
    ) : IPublishOrchestrator
{
    private readonly IPublishExecutor[] _executors = [.. executors];

    public ValueTask PublishAsync(PublishContent content, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();

        foreach (var executor in _executors)
        {
            var task = executor.ExecuteAsync(content, cancellationToken).AsTask();
            tasks.Add(task);
        }

        return new ValueTask(Task.WhenAll(tasks));
    }


    [LoggerMessage(Level = LogLevel.Information, Message = "处理完成 < 来源 [{Source}]")]
    private partial void LogFinish(ISource source);
}
