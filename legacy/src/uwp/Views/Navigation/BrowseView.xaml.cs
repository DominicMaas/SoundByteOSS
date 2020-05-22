using SoundByte.App.Uwp.ViewModels.Navigation;

namespace SoundByte.App.Uwp.Views.Navigation
{
    public sealed partial class BrowseView
    {
        public BrowseViewModel ViewModel => (BrowseViewModel)DataContext;

        public BrowseView() => InitializeComponent();
    }
}