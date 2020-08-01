using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.ViewModels.Details;

namespace SoundByte.App.UWP.Views.Details
{
    [MvxViewFor(typeof(PodcastDetailViewModel))]
    public sealed partial class PodcastDetailView : MvxWindowsPage
    {
        public PodcastDetailViewModel Vm => (PodcastDetailViewModel)ViewModel;

        public PodcastDetailView() => InitializeComponent();
    }
}
