using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.UWP.Views.Main
{
    [SoundByteTab("podcasts")]
    [MvxViewFor(typeof(PodcastsViewModel))]
    public sealed partial class PodcastsView : MvxWindowsPage
    {
        public PodcastsViewModel Vm => (PodcastsViewModel)ViewModel;

        public PodcastsView() => InitializeComponent();
    }
}