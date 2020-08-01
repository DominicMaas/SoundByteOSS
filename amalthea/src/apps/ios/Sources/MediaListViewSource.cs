using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.App.iOS.Cells.Media.List;
using SoundByte.Core.Helpers;
using SoundByte.Core.Models.Media;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Sources
{
    public class MediaListViewSource : MvxTableViewSource
    {
        public MediaListViewSource(UITableView tableView) : base(tableView)
        {
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.AllowsSelection = true;
            TableView.AllowsMultipleSelection = false;
            TableView.RowHeight = MediaListCell.CellHeight;
            //
            // TableView.BackgroundColor = ColorHelper.Background0.ToPlatformColor();
            // TableView.SectionIndexBackgroundColor = ColorHelper.Background0.ToPlatformColor();
            //TableView.SectionIndexTrackingBackgroundColor = ColorHelper.Background0.ToPlatformColor();

            TableView.RegisterClassForCellReuse(typeof(TrackListCell), TrackListCell.Key);
            TableView.RegisterClassForCellReuse(typeof(PlaylistListCell), PlaylistListCell.Key);
            TableView.RegisterClassForCellReuse(typeof(UserListCell), UserListCell.Key);
            TableView.RegisterClassForCellReuse(typeof(PodcastShowListCell), PodcastShowListCell.Key);
        }

        protected override UITableViewCell GetOrCreateCellFor(UITableView tableView, NSIndexPath indexPath, object item)
        {
            // Get the cell
            var cell = item switch
            {
                Track _ => tableView.DequeueReusableCell(TrackListCell.Key, indexPath),
                Playlist _ => tableView.DequeueReusableCell(PlaylistListCell.Key, indexPath),
                User _ => tableView.DequeueReusableCell(UserListCell.Key, indexPath),
                PodcastShow _ => tableView.DequeueReusableCell(PodcastShowListCell.Key, indexPath),
                _ => null
            };

            // No Cell
            if (cell == null) return GetOrCreateCellFor(tableView, indexPath, item);

            // Set the background color and return
            cell.BackgroundColor = ColorHelper.Background0.ToPlatformColor();
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }
}