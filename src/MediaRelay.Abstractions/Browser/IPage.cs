namespace MediaRelay.Browser;

public interface IPage : IAsyncDisposable
{
    Task GotoAsync(string url, PageGotoOptions? options = null);
    Task<T> EvaluateAsync<T>(string expression, object? arg = default);
}
