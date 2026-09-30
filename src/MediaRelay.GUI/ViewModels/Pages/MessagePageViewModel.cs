using Avalonia.Collections;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaRelay.Source;

namespace MediaRelay.Desktop.ViewModels.Pages;


internal sealed class MessagePageViewModel : ObservableObject
{
    public AvaloniaList<MessageViewModel> MessageItemsSource { get; } = [];


    public MessagePageViewModel()
    {
        //if (!Design.IsDesignMode) return;

        var vm = RelayInfoViewModel.CreateAsync(
        [
             new(@"D:\Src\Data\Code\VisualStudio\DaThabe\MediaRelay\src\MediaRelay.Launcher.Console\bin\Debug\net10.0\Storage\8ff3a0bafd2c7a4bb0b6306c1877971420cb3253fb7fe2f667ff5a49082aa5a5.jpg"),
             new(@"D:\Src\Data\Code\VisualStudio\DaThabe\MediaRelay\src\MediaRelay.Launcher.Console\bin\Debug\net10.0\Storage\342b5ebd56357637f945de50f4906a1ad9b6199635ebf923067b3f671a33100c.jpg"),
             new(@"D:\Src\Data\Code\VisualStudio\DaThabe\MediaRelay\src\MediaRelay.Launcher.Console\bin\Debug\net10.0\Storage\440302676a4e40e7f758c081c4dbd8a5373e61f40e20926aa2e1f883216f0184.jpg"),
        ]).GetAwaiter().GetResult();

        vm.Title = "标题";
        vm.Description = "描述";
        vm.PublishAt = DateTimeOffset.Now;
        vm.Tags = ["C#", "Avalonia", "MediaRelay", "Windows-Desktop"];

        MessageItemsSource =
        [
            new()
            {
                SourceId = SourceId.Create("Pixiv:123456789"),
                CreateAt = DateTimeOffset.Now,
                InputType = "Clipboard",
                Info = vm
            }
        ];
    }
}


public sealed partial class MessageViewModel : ObservableObject
{
    [ObservableProperty]
    public partial DateTimeOffset CreateAt { get; set; }

    [ObservableProperty]
    public partial string InputType { get; set; }

    [ObservableProperty]
    public partial SourceId SourceId { get; set; }

    [ObservableProperty]
    public required partial RelayInfoViewModel Info { get; set; }


    [RelayCommand]
    private async Task RetryAsync()
    {

    }
}

public sealed class RelayInfoViewModel : ObservableObject
{
    public required IReadOnlyList<Bitmap> Resources { get; init; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;


    public AvaloniaList<string> Tags { get; set; } = [];

    public DateTimeOffset PublishAt { get; set; }


    public static async ValueTask<RelayInfoViewModel> CreateAsync(IReadOnlyCollection<Uri> resourceUris)
    {
        if (resourceUris.Count == 0)
            throw new InvalidOperationException("没有资源");

        var bitmaps = new List<Bitmap>();

        foreach (var i in resourceUris)
        {
            if (i.IsFile)
            {
                var bitmap = new Bitmap(i.LocalPath);
                bitmaps.Add(bitmap);
            }
        }

        return new() { Resources = bitmaps };
    }
}