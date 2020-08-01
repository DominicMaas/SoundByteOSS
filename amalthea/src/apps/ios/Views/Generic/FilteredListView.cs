using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Generic;
using System.Threading;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Generic
{
    public class FilteredListView : MvxTableViewController<FilteredListViewModel>, IUITableViewDataSourcePrefetching
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Create table source
            var source = new FilteredListViewSource(TableView);

            // Init bindings
            var set = this.CreateBindingSet<FilteredListView, FilteredListViewModel>();
            set.Bind(this).For(x => x.Title).To(vm => vm.Title);
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.Model);
            set.Apply();

            // Load Content
            TableView.PrefetchDataSource = this;
            TableView.Source = source;
            TableView.ReloadData();
        }

        public class FilteredListViewSource : MvxStandardTableViewSource
        {
            public FilteredListViewSource(UITableView tableView)
                : base(tableView, UITableViewCellStyle.Subtitle, new NSString("FilteredListView"), "TitleText Title;DetailText User.Username", UITableViewCellAccessory.DisclosureIndicator)
            {
                tableView.AllowsSelection = true;
                tableView.AllowsMultipleSelection = false;
            }
        }

        public async void PrefetchRows(UITableView tableView, NSIndexPath[] indexPaths)
        {
            await ViewModel.Model.LoadMoreItemsAsync(25, CancellationToken.None);
        }
    }
}