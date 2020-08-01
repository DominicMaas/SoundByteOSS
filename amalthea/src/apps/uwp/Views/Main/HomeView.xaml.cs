using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.UWP.Views.Main
{
    [SoundByteTab("home")]
    [MvxViewFor(typeof(HomeViewModel))]
    public sealed partial class HomeView : MvxWindowsPage
    {
        public HomeViewModel Vm => (HomeViewModel)ViewModel;

        public HomeView() => InitializeComponent();
    }
}