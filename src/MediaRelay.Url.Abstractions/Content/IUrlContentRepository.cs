using MediaRelay.Source;

namespace MediaRelay.Content;

public interface IUrlContentRepository
{
    ValueTask<IUrlContent?> FindAsync(SourceId sourceId, CancellationToken cancellationToken = default);
    ValueTask SetAsync(IUrlContent content, CancellationToken cancellationToken = default);
}


//IUrlContentRepository contentRepository;

//public override async ValueTask<DefaultUrlContent> ExtractAsync(TUrlSource source, CancellationToken cancellationToken = default)
//{
//    if (source is not IUrlSource urlSource)
//        throw new NotSupportedException($"不支持的非网址来源: {source}");

//    // 加载
//    var content = await contentRepository.FindAsync(urlSource.Id, cancellationToken);
//    if (content is not null) return Parse(content);

//    // 储存
//    content = await base.ExtractAsync(source, cancellationToken);
//    await contentRepository.SetAsync(content, cancellationToken);

//    // 返回
//    return Parse(content);
//}


//IUrlContentRepository contentRepository;

//public override async ValueTask<DefaultUrlContent> ExtractAsync(TUrlSource source, CancellationToken cancellationToken = default)
//{
//    if (source is not IUrlSource urlSource)
//        throw new NotSupportedException($"不支持的非网址来源: {source}");

//    // 加载
//    var content = await contentRepository.FindAsync(urlSource.Id, cancellationToken);
//    if (content is not null) return Parse(content);

//    // 储存
//    content = await base.ExtractAsync(source, cancellationToken);
//    await contentRepository.SetAsync(content, cancellationToken);

//    // 返回
//    return Parse(content);
//}