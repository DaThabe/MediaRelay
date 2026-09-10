namespace MediaRelay.Url;


/// <summary>
/// 根据网址创建网址来源
/// </summary>
public interface IUrlSourceFactory
{
    IUrlSource Create(Uri uri);
}