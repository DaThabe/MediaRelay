namespace MediaRelay.Extensions;

public static class SemaphoreSlimExtensions
{
    extension(SemaphoreSlim semaphoreSlim)
    {
        public async Task<IDisposable> WaitScopeAsync(CancellationToken cancellationToken = default)
        {
            await semaphoreSlim.WaitAsync(cancellationToken);
            return new Releaser(semaphoreSlim);
        }
    }
}

file sealed class Releaser(SemaphoreSlim semaphoreSlim) : IDisposable
{
    public void Dispose() => semaphoreSlim.Release();
}
