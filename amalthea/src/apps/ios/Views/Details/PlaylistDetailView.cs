using System.Threading;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Details;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Details
{
    public class PlaylistDetailView : MvxTableViewController<PlaylistDetailViewModel>, IUITableViewDataSourcePrefetching
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Build the header
            var header = new UIView();

            var title = new UILabel();

            // Add content to header
            header.Add(title);

            // Constraints

            // Set the header
            TableView.TableHeaderView = header;

            // Create table source
            var source = new MediaListViewSource(TableView);

            // Init bindings
            var set = this.CreateBindingSet<PlaylistDetailView, PlaylistDetailViewModel>();
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.Tracks);
            set.Bind(source).For(x => x.SelectionChangedCommand).To(vm => vm.PlayItemCommand);
            set.Bind(title).For(x => x.Text).To(vm => vm.Playlist.Title);
            set.Apply();

            // Load Content
            TableView.PrefetchDataSource = this;
            TableView.Source = source;
            TableView.ReloadData();
        }

        public async void PrefetchRows(UITableView tableView, NSIndexPath[] indexPaths)
        {
            await ViewModel.Tracks.LoadMoreItemsAsync(25, CancellationToken.None);
        }
    }
}