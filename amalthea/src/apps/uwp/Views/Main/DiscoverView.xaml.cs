using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.UWP.Views.Main
{
    [SoundByteTab("discover")]
    [MvxViewFor(typeof(DiscoverViewModel))]
    public sealed partial class DiscoverView : MvxWindowsPage
    {
        public DiscoverViewModel Vm => (DiscoverViewModel)ViewModel;

        public DiscoverView() => InitializeComponent();
    }
}