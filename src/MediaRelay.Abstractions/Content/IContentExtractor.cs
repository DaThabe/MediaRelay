using MediaRelay.Source;

namespace MediaRelay.Content;

/// <summary>
/// 从来源提取内容
/// </summary>
public interface IContentExtractor
{
    bool CanExtract(ISource source);
    ValueTask<IContent> ExtractAsync(ISource source, CancellationToken cancellationToken = default);
}