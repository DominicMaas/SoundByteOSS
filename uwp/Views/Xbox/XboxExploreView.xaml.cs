using SoundByte.App.Uwp.ViewModels.Navigation;

namespace SoundByte.App.Uwp.Views.Xbox
{
    public sealed partial class XboxExploreView
    {
        public BrowseViewModel ViewModel => (BrowseViewModel)DataContext;

        public XboxExploreView() => InitializeComponent();
    }
}