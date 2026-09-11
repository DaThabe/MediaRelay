using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace MediaRelay.Browser;

public interface IPage : IAsyncDisposable
{
    ValueTask GotoAsync(string url, PageGotoOptions? options = null, CancellationToken cancellationToken = default);
    ValueTask<JsonElement?> EvaluateAsync(string expression, object? arg = default, CancellationToken cancellationToken = default);
    ValueTask<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        string expression, object? arg = default, CancellationToken cancellationToken = default);
}


public static class PageExtensions
{
    extension(IPage page)
    {
        public async ValueTask<T> EvaluateScriptFileAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string scriptPath, object? arg = null, CancellationToken cancellationToken = default)
        {
            if(!File.Exists(scriptPath))
                throw new FileNotFoundException($"脚本文件不存在", scriptPath);

            var script = await File.ReadAllTextAsync(scriptPath, cancellationToken);
            return await page.EvaluateAsync<T>(script, arg);
        }
    }
}