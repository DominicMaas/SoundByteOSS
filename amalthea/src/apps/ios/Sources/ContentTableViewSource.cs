using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using SoundByte.App.iOS.Cells;
using SoundByte.Core.Helpers;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Sources
{
    public class ContentTableViewSource : MvxSimpleTableViewSource
    {
        public ContentTableViewSource(UITableView tableView) : base(tableView, ContentGroupCell.Key, ContentGroupCell.Key)
        {
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.AllowsSelection = false;
            TableView.AllowsMultipleSelection = false;
            TableView.RowHeight = ContentGroupCell.CellHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = base.GetCell(tableView, indexPath);
            cell.BackgroundColor = ColorHelper.Background0.ToPlatformColor();
            return cell;
        }
    }
}