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
    public class AccountsView : MvxTableViewController<AccountsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Manage Accounts";
            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Create table source
            var source = new AccountsTableViewSource(TableView);
            TableView.Source = source;

            // Init bindings
            var set = this.CreateBindingSet<AccountsView, AccountsViewModel>();
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.MusicProviderAccounts);
            set.Bind(source).For(x => x.SelectionChangedCommand).To(vm => vm.InvokeCommand);
            set.Apply();

            // Load Content
            TableView.ReloadData();

            // Cancel button
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, delegate { ViewModel.CloseCommand.Execute(); });
        }

        public async override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            await ViewModel.CloseCommand.ExecuteAsync();
        }

        public class AccountsTableViewSource : MvxStandardTableViewSource
        {
            public AccountsTableViewSource(UITableView tableView)
                : base(tableView, UITableViewCellStyle.Subtitle, new NSString("MusicProviderAccountsListView"), "TitleText Name;DetailText ConnectedStatus", UITableViewCellAccessory.DisclosureIndicator)
            {
                tableView.AllowsSelection = true;
                tableView.AllowsMultipleSelection = false;
            }

            // public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            // {
            //     tableView.DeselectRow(indexPath, true);
            // }
        }
    }
}