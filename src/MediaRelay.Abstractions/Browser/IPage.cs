using MediaRelay.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Browser;

public interface IPage : IAsyncDisposable
{
    bool IsClosed { get; }
    ValueTask CloseAsync();


    ValueTask GotoAsync(string url, PageGotoOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        string expression, object? arg = default, CancellationToken cancellationToken = default);

    ValueTask<T> EvaluateAsync<T>(string expression, object? arg, ISerializer<T> serializer, CancellationToken cancellationToken = default)
        where T : notnull;
}


public static class PageExtensions
{
    extension(IPage page)
    {
        public async ValueTask<T> EvaluateScriptFileAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string scriptPath, object? arg = null, CancellationToken cancellationToken = default)
        {
            var script = await GetScriptFileAllTextAsync(scriptPath, cancellationToken);
            return await page.EvaluateAsync<T>(script, arg, cancellationToken);
        }

        public async ValueTask<T> EvaluateScriptFileAsync<T>(string scriptPath, object? arg, ISerializer<T> serializer, CancellationToken cancellationToken = default)
            where T : notnull
        {
            var script = await GetScriptFileAllTextAsync(scriptPath, cancellationToken);
            return await page.EvaluateAsync(script, arg, serializer, cancellationToken);
        }
    }


    private static async Task<string> GetScriptFileAllTextAsync(string path, CancellationToken cancellationToken)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"脚本文件不存在", path);

        var text = await File.ReadAllTextAsync(path, cancellationToken);

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("脚本文件为空", nameof(path));

        return text;
    }
}