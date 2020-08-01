using Windows.UI.Xaml.Controls;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.Core.Models.Media;
using SoundByte.Core.ViewModels.Details;

namespace SoundByte.App.UWP.Views.Details
{
    [MvxViewFor(typeof(PlaylistDetailViewModel))]
    public sealed partial class PlaylistDetailView : MvxWindowsPage
    {
        public PlaylistDetailViewModel Vm => (PlaylistDetailViewModel)ViewModel;

        public PlaylistDetailView() => InitializeComponent();

        private async void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            await Vm.PlayItemCommand.ExecuteAsync(e.ClickedItem as Media);
        }
    }
}
