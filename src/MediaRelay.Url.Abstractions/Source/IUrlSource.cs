namespace MediaRelay.Source;

/// <summary>
/// 表示一个网址来源
/// </summary>
public interface IUrlSource : ISource
{
    Uri Url { get; }
}