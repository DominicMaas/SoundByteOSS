using SoundByte.App.Uwp.ViewModels.Panes;

namespace SoundByte.App.Uwp.Views.Panes
{
    public sealed partial class MusicProvidersPaneView
    {
        public ExtensionPaneViewModel ViewModel => (ExtensionPaneViewModel)DataContext;

        public MusicProvidersPaneView() => InitializeComponent();
    }
}