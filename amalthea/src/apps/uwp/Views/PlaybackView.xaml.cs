using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels;

namespace SoundByte.App.UWP.Views
{
    [MvxViewFor(typeof(PlaybackViewModel))]
    public sealed partial class PlaybackView : MvxWindowsPage
    {
        public PlaybackViewModel Vm => (PlaybackViewModel)ViewModel;

        public PlaybackView() => InitializeComponent();
    }
}
