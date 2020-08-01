using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.iOS.Views.Main
{
    [MvxTabPresentation(WrapInNavigationController = true, TabIconName = "podcasts", TabName = "Podcasts")]
    public class PodcastsView : MvxTableViewController<PodcastsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Podcasts";
            NavigationController.NavigationBar.PrefersLargeTitles = true;

            // Create refresh control
            var refreshControl = new MvxUIRefreshControl();
            RefreshControl = refreshControl;

            // Create table source
            var source = new ContentTableViewSource(TableView);

            // Init bindings
            var set = this.CreateBindingSet<PodcastsView, PodcastsViewModel>();
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