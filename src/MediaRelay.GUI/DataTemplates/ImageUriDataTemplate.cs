using AsyncImageLoader;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace MediaRelay.GUI.DataTemplates;


/// <summary>
/// 图像网址转为Image
/// </summary>
internal sealed class ImageUriDataTemplate : IDataTemplate
{
    public bool Match(object? data)
    {
        return data is Uri;
    }

    public Control? Build(object? param)
    {
        if (param is not Uri uri) return default;

        var img = new Image();
        var cts = new CancellationTokenSource();

        img.Unloaded += delegate
        {
            cts.Cancel();
            cts.Dispose();

            var source = img.Source;
            img.Source = null;
            if (source is IDisposable disposable) disposable.Dispose();
        };

        _ = LoadImageAsync(img, uri, cts.Token);

        return img;
    }

    private static async Task LoadImageAsync(Image target, Uri uri, CancellationToken token)
    {
        try
        {
            var bitmap = await ImageLoader.AsyncImageLoader.ProvideImageAsync(uri.ToString());

            // 加载期间控件可能已被卸载，或被取消了
            if (token.IsCancellationRequested)
            {
                bitmap?.Dispose(); // 已卸载，没人接手，直接释放
                return;
            }

            // 续体默认回到 UI 线程（方法由 UI 线程发起）
            target.Source = bitmap;
        }
        catch (OperationCanceledException)
        {
            // 正常取消，忽略
        }
    }
}