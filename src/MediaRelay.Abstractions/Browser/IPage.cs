using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Browser;

public interface IPage : IAsyncDisposable
{
    Task GotoAsync(string url, PageGotoOptions? options = null, CancellationToken cancellationToken = default);
    Task<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string expression, object? arg = default);
}


public static class PageExtensions
{
    extension(IPage page)
    {
        public async Task<string> EvaluateScriptFileAsync(string scriptPath, object? arg = null, CancellationToken cancellationToken = default)
        {
            if(!File.Exists(scriptPath))
                throw new FileNotFoundException($"脚本文件不存在", scriptPath);

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            return await page.EvaluateAsync<string>(script, arg);
        }
    }
}