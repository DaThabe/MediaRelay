namespace MediaRelay.Source;


/// <summary>
/// 表示内容来源
/// </summary>
public interface ISource
{
    SourceId Id { get; }
}

/// <summary>
/// 表示一个网址来源
/// </summary>
public interface IUrlSource : ISource
{
    Uri Url { get; }
}