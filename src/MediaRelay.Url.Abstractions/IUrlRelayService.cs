namespace MediaRelay;


public interface IUrlRelayService
{
    /// <summary>
    /// 从 Url 转发
    /// </summary>
    ValueTask RelayAsync(Uri url, CancellationToken cancellationToken = default);
}