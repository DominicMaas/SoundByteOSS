using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.UWP.Views.Main
{
    [SoundByteTab("me")]
    [MvxViewFor(typeof(MeViewModel))]
    public sealed partial class MeView : MvxWindowsPage
    {
        public MeViewModel Vm => (MeViewModel)ViewModel;

        public MeView() => InitializeComponent();
    }
}