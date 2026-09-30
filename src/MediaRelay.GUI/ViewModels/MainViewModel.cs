using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using MediaRelay.GUI.ViewModels.Pages;

namespace MediaRelay.GUI.ViewModels;


internal partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public partial MessagePageViewModel? MessagePage { get; set; }


    public MainViewModel()
    {
        if (!Design.IsDesignMode) return;

        MessagePage = new();
    }
}