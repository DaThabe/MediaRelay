using MediaRelay.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace MediaRelay.Pixiv;

public interface IPixivDownloader
{
    ValueTask<Stream> DownloadAsync(string url, CancellationToken cancellationToken = default);
}

internal sealed class PixivDownloader(IOptions<PixivOptions> options, IHttpClient httpClient, ILogger<PixivDownloader> logger) : IPixivDownloader
{
    private readonly SemaphoreSlim _lock = new(options.Value.Http.MaxConcurrentDownloads, options.Value.Http.MaxConcurrentDownloads);
    private readonly ResiliencePipeline<Stream> pipeline = new ResiliencePipelineBuilder<Stream>()
        .AddRetry(new()
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential, // 指数退避
            UseJitter = true, // 增加抖动，避免同时重试导致服务压力过大[citation:3][citation:7]
            Delay = TimeSpan.FromSeconds(2), // 基础延迟2秒
            ShouldHandle = new PredicateBuilder<Stream>()
            .Handle<HttpRequestException>()
        })
        .Build();

    public async ValueTask<Stream> DownloadAsync(string url, CancellationToken cancellationToken = default)
    {
        await _lock.WaitAsync(cancellationToken);

        try
        {
            return await pipeline.ExecuteAsync(Handler, cancellationToken);
        }
        finally
        {
            _lock.Release();
        }


        async ValueTask<Stream> Handler(CancellationToken cancellationToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Referrer = new Uri(options.Value.Http.Referrer);

                logger.LogDebug("下载中");

                var response = await httpClient.SendAsync(request, cancellationToken);
                //response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                logger.LogInformation("下载完成");

                return stream;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "下载失败");
                throw;
            }
        }
    }
}