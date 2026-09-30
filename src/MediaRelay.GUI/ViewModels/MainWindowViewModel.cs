using CommunityToolkit.Mvvm.ComponentModel;

namespace MediaRelay.GUI.ViewModels;


internal sealed class MainWindowViewModel : ObservableObject
{
    public required MainViewModel MainView { get; init; }
}