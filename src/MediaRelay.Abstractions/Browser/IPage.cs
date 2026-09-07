namespace MediaRelay.Browser;

public interface IPage : IAsyncDisposable
{
    Task GotoAsync(string url, PageGotoOptions? options = null);
    Task<T> EvaluateAsync<T>(string expression, object? arg = default);
}


public static class PageExtensions
{
    extension(IPage page)
    {
        public async Task<string> EvaluateScriptAsync(string scriptPath, object? arg = null, CancellationToken cancellationToken = default)
        {
            if(!File.Exists(scriptPath))
                throw new FileNotFoundException($"脚本文件不存在", scriptPath);

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            if (string.IsNullOrWhiteSpace(script)) return string.Empty;

            return await page.EvaluateAsync<string>(script, arg);
        }
    }
}