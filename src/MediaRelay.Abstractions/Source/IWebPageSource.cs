namespace MediaRelay.Source;

/// <summary>
/// 表示一个网站来源
/// </summary>
public interface IWebPageSource : ISource
{
    Uri Url { get; }
}