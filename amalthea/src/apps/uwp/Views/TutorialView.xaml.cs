using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels;

namespace SoundByte.App.UWP.Views
{
    [MvxViewFor(typeof(TutorialViewModel))]
    public sealed partial class TutorialView : MvxWindowsPage
    {
        public TutorialViewModel Vm => (TutorialViewModel)ViewModel;

        public TutorialView() => InitializeComponent();
    }
}
