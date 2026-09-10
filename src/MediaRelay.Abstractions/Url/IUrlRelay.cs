namespace MediaRelay.Url;

/// <summary>
/// 从 Url 转发
/// </summary>
public interface IUrlRelay
{
    ValueTask RelayAsync(Uri url, CancellationToken cancellationToken = default);
}