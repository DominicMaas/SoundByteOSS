using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.ViewModels.Main;
using UIKit;

namespace SoundByte.App.iOS.Views.Main
{
    [MvxTabPresentation(WrapInNavigationController = true, TabIconName = "home", TabName = "Home")]
    public class HomeView : MvxTableViewController<HomeViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Home";
            NavigationController.NavigationBar.PrefersLargeTitles = true;
            NavigationItem.LargeTitleDisplayMode = UINavigationItemLargeTitleDisplayMode.Always;

            // Create refresh control
            var refreshControl = new MvxUIRefreshControl();
            TableView.RefreshControl = refreshControl;

            // Create the refresh button
            var refreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh);
            NavigationItem.RightBarButtonItem = refreshButton;

            // Create table source
            var source = new ContentTableViewSource(TableView);

            // Init bindings
            var set = this.CreateBindingSet<HomeView, HomeViewModel>();
            set.Bind(refreshButton).To(vm => vm.RefreshCommand);
            set.Bind(refreshControl).For(x => x.RefreshCommand).To(vm => vm.RefreshCommand);
            set.Bind(refreshControl).For(x => x.IsRefreshing).To(vm => vm.IsLoading);
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.PageContent);
            set.Apply();

            // Load Content
            TableView.Source = source;
            TableView.ReloadData();
        }
    }
}