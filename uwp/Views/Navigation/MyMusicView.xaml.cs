using SoundByte.App.Uwp.ViewModels.Navigation;

namespace SoundByte.App.Uwp.Views.Navigation
{
    public sealed partial class MyMusicView
    {
        public MyMusicViewModel ViewModel => (MyMusicViewModel)DataContext;

        public MyMusicView() => InitializeComponent();
    }
}