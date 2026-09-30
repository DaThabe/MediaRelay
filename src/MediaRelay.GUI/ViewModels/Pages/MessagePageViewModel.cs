using Avalonia.Collections;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediaRelay.Source;

namespace MediaRelay.GUI.ViewModels.Pages;


internal sealed class MessagePageViewModel : ObservableObject
{
    public AvaloniaList<MessageViewModel> ItemsSource { get; } = [];


    public MessagePageViewModel()
    {
        if (!Design.IsDesignMode) return;

        var vm = new RelayInfoViewModel
        {
            Title = "这是一个标题",
            Description = "这是一段很长很长的描述, 真的很长很长很长很长很长很长很长很长很长很长很长, 到这里依然没有结束, 再来一段吧.",
            PublishAt = DateTimeOffset.Now,
            Tags = ["C#", "Avalonia", "MediaRelay", "Windows-Desktop", "GUI", "ViewModels", "Pages", "Url", "Clipboard", "HeyBox", "Immich", "Pixiv", "Immich", "Twitter"],
            Resources =
            [
                new Uri("https://gd-hbimg-edge.huaban.com/9bd606efa293bcae22557db356b5bd9a407128839f76-ugolUR_fw480webp?auth_key=1790798400-99becdca6a5a401793117ec5eab127d0-0-4340e6936980b80ade8130dc3cc74b67"),
                new Uri("https://gd-hbimg-edge.huaban.com/1c7c90fb63794a5975c6c2095713ce6e91dae58d1230a-JKztaz_fw480webp?auth_key=1790798400-99becdca6a5a401793117ec5eab127d0-0-d60c8c55a94efd9131f691650c0d4e49"),
                new Uri("https://gd-hbimg-edge.huaban.com/b462becc364c2337f6b90b540d2ada359a2e87cd36222-yM9nxo_fw480webp?auth_key=1790798400-99becdca6a5a401793117ec5eab127d0-0-170324b0e320b8fd464bf58efd57952d"),
                new Uri("https://gd-hbimg-edge.huaban.com/77b561783f9998a4a68dccbd1d537ccb8a3245ed1df14-s5fPEG_fw480webp?auth_key=1790798400-99becdca6a5a401793117ec5eab127d0-0-dfb1a9c680444a27fa393ed2b74a71a8")
            ]
        };

        ItemsSource =
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
    public required IReadOnlyList<Uri> Resources { get; init; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;


    public AvaloniaList<string> Tags { get; set; } = [];

    public DateTimeOffset PublishAt { get; set; }
}