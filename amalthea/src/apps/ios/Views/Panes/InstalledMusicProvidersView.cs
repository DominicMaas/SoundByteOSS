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
    public class InstalledMusicProvidersView : MvxTableViewController<InstalledMusicProvidersViewModel>
    {
        private UIBarButtonItem editButton;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Installed";
            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Create table source
            var source = new InstalledMusicProvidersTableViewSource(TableView, ViewModel);

            // Init bindings
            var set = this.CreateBindingSet<InstalledMusicProvidersView, InstalledMusicProvidersViewModel>();
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.MusicProviders);
            set.Apply();

            // Load Content
            TableView.Source = source;
            TableView.ReloadData();

            // Create edit button
            editButton = new UIBarButtonItem("Manage", UIBarButtonItemStyle.Done, delegate
            {
                TableView.SetEditing(!TableView.Editing, true);
                editButton.Title = TableView.Editing ? "Done" : "Manage";
            });

            NavigationItem.RightBarButtonItem = editButton;

            // Cancel button
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, delegate { ViewModel.CloseCommand.Execute(); });
        }

        public async override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            await ViewModel.CloseCommand.ExecuteAsync();
        }

        public class InstalledMusicProvidersTableViewSource : MvxStandardTableViewSource
        {
            private readonly InstalledMusicProvidersViewModel _viewModel;

            public InstalledMusicProvidersTableViewSource(UITableView tableView, InstalledMusicProvidersViewModel vm)
                : base(tableView, UITableViewCellStyle.Subtitle, new NSString("InstalledMusicProvidersListView"), "TitleText Manifest.Name;DetailText Manifest.Publisher", UITableViewCellAccessory.DisclosureIndicator)
            {
                _viewModel = vm;
                tableView.AllowsSelection = false;
                tableView.AllowsMultipleSelection = false;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                return true;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                switch (editingStyle)
                {
                    case UITableViewCellEditingStyle.Delete:
                        _viewModel.UninstallCommand.Execute(_viewModel.MusicProviders[indexPath.Row]);
                        break;

                    case UITableViewCellEditingStyle.None:
                        break;
                }
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

            public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
            {
                return false;
            }
        }
    }
}