using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.Helpers;
using SoundByte.Core.ViewModels.Generic;
using System.Threading;
using UIKit;
using Xamarin.Essentials;

namespace SoundByte.App.iOS.Views.Generic
{
    public class GenericListView : MvxTableViewController<GenericListViewModel>, IUITableViewDataSourcePrefetching
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.PrefersLargeTitles = true;
            View.BackgroundColor = ColorHelper.Background0.ToPlatformColor();

            // Create table source
            var source = new MediaListViewSource(TableView);

            // Init bindings
            var set = this.CreateBindingSet<GenericListView, GenericListViewModel>();
            set.Bind(this).For(x => x.Title).To(vm => vm.Title);
            set.Bind(source).For(x => x.ItemsSource).To(vm => vm.Model);
            set.Bind(source).For(x => x.SelectionChangedCommand).To(vm => vm.InvokeCommand);
            set.Apply();

            // Load Content
            TableView.PrefetchDataSource = this;
            TableView.Source = source;
            TableView.ReloadData();
        }

        public async void PrefetchRows(UITableView tableView, NSIndexPath[] indexPaths)
        {
            await ViewModel.Model.LoadMoreItemsAsync(25, CancellationToken.None);
        }
    }
}