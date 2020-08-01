using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Panes;

namespace SoundByte.App.UWP.Views.Panes
{
    [SoundByteModal("SoundByte Premium")]
    [MvxViewFor(typeof(PremiumViewModel))]
    public sealed partial class PremiumView : MvxWindowsPage
    {
        public PremiumViewModel Vm => (PremiumViewModel)ViewModel;

        public PremiumView() => InitializeComponent();
    }
}
