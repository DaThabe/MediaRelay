using CommunityToolkit.Mvvm.ComponentModel;
using MediaRelay.Desktop.ViewModels.Pages;

namespace MediaRelay.GUI.ViewModels;


internal partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public partial MessagePageViewModel? MessagePage { get; set; }
}