using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.UWP.Views.Main
{
    [SoundByteTab("my-music")]
    [MvxViewFor(typeof(MyMusicViewModel))]
    public sealed partial class MyMusicView : MvxWindowsPage
    {
        public MyMusicViewModel Vm => (MyMusicViewModel)ViewModel;

        public MyMusicView() => InitializeComponent();
    }
}