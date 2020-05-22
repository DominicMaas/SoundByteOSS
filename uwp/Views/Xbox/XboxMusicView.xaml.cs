using SoundByte.App.Uwp.ViewModels.Xbox;

namespace SoundByte.App.Uwp.Views.Xbox
{
    public sealed partial class XboxMusicView
    {
        public XboxMusicViewModel ViewModel => (XboxMusicViewModel)DataContext;

        public XboxMusicView() => InitializeComponent();
    }
}