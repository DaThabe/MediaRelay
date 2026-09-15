using MediaRelay.Serializer;
using System.Diagnostics.CodeAnalysis;

namespace MediaRelay.Browser.Shared;


[Obsolete]
internal sealed class SharedPage(IPage inner) : IPage
{
    public bool IsClosed => inner.IsClosed;

    public ValueTask<T> EvaluateAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string expression, object? arg = null, CancellationToken cancellationToken = default)
        => inner.EvaluateAsync<T>(expression, arg, cancellationToken);

    public ValueTask<T> EvaluateAsync<T>(string expression, object? arg, ISerializer<T> serializer, CancellationToken cancellationToken = default) where T : notnull
        => inner.EvaluateAsync(expression, arg, serializer, cancellationToken);

    public ValueTask GotoAsync(string url, PageGotoOptions? options = null, CancellationToken cancellationToken = default)
        => inner.GotoAsync(url, options, cancellationToken);

    public ValueTask CloseAsync()
        => inner.CloseAsync();

    public ValueTask DisposeAsync()
       => ValueTask.CompletedTask;
}