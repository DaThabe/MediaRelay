using Avalonia.Controls;
using MediaRelay.GUI.DataTemplates;

namespace MediaRelay.GUI.Views.Pages;


internal partial class MessagePage : UserControl
{
    public MessagePage()
    {
        InitializeComponent();
        DataTemplates.Add(new ImageUriDataTemplate());
    }
}