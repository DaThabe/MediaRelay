namespace MediaRelay.Source;


/// <summary>
/// 表示内容来源
/// </summary>
public interface ISource
{
    SourceId Id { get; }
}

public record class DefaultSource : ISource
{
    public required SourceId Id { get; init; }
}