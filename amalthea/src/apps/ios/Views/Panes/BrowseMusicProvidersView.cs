using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Panes;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Panes
{
    [MvxModalPresentation(ModalPresentationStyle = UIModalPresentationStyle.Popover, WrapInNavigationController = true)]
    public partial class BrowseMusicProvidersView : MvxTableViewController<BrowseMusicProvidersViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Browse";
            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Create a search controller
            var search = new UISearchController(searchResultsController: null)
            {
                DimsBackgroundDuringPresentation = true
            };

            // Set the search controller
            NavigationItem.SearchController = search;

            // Create table source
            var source = new BrowseMusicProvidersTableViewSource(TableView);

            // Init bindings
            var set = this.CreateBindingSet<BrowseMusicProvidersView, BrowseMusicProvidersViewModel>();
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.MusicProviders);
            set.Bind(source).For(x => x.SelectionChangedCommand).To(vm => vm.InstallCommand);
            set.Apply();

            // Load Content
            TableView.Source = source;
            TableView.ReloadData();

            // Cancel button
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, delegate { ViewModel.CloseCommand.Execute(); });
        }

        public override async void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            await ViewModel.CloseCommand.ExecuteAsync();
        }

        public class BrowseMusicProvidersTableViewSource : MvxStandardTableViewSource
        {
            public BrowseMusicProvidersTableViewSource(UITableView tableView)
                : base(tableView, UITableViewCellStyle.Subtitle, new NSString("BrowseMusicProvidersListView"), "TitleText Name;DetailText Publisher", UITableViewCellAccessory.DisclosureIndicator)
            {
                tableView.AllowsSelection = true;
                tableView.AllowsMultipleSelection = false;
            }

            //public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            //{
            //    tableView.DeselectRow(indexPath, true);
            //}
        }
    }
}