using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Binding.Views;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Controls;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.ViewModels;

namespace SoundByte.App.iOS.Views
{
    public class SearchView : MvxTableViewController<SearchViewModel>
    {
        private MvxTableViewSource _source;
        private MvxUIRefreshControl _refreshControl;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.NavigationBar.PrefersLargeTitles = true;

            // Create refresh control
            _refreshControl = new MvxUIRefreshControl();
            RefreshControl = _refreshControl;

            // Create a segmented control (split between likes, and playlists)
            var segmentedControl = new SoundByteSegmentedControl(new[] { "Tracks", "Playlists", "Users", "Podcasts" });
            segmentedControl.IndexChanged += OnSegmentedControlIndexChanged;
            TableView.TableHeaderView = segmentedControl;

            // Initial selection
            OnSegmentedControlIndexChanged(this, 0);
        }

        private void OnSegmentedControlIndexChanged(object sender, int index)
        {
            // Ensure the correct source is set (the podcast uses a list vs the music search
            // results which uses a list of content groups).
            if (index == 3)
            {
                if (_source is ContentTableViewSource || _source is null)
                {
                    _source = new MediaListViewSource(TableView);
                    TableView.Source = _source;
                }
            }
            else
            {
                if (_source is MediaListViewSource || _source is null)
                {
                    _source = new ContentTableViewSource(TableView);
                    TableView.Source = _source;
                }
            }

            var set = this.CreateBindingSet<SearchView, SearchViewModel>();
            set.Bind(_refreshControl).For(x => x.RefreshCommand).To(vm => vm.RefreshCommand);
            set.Bind(_refreshControl).For(x => x.IsRefreshing).To(vm => vm.IsLoading);
            set.Bind(this).For(x => x.Title).To(vm => vm.SearchText);

            switch (index)
            {
                case 0:
                    set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.TracksContent);
                    break;

                case 1:
                    set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.PlaylistsContent);
                    break;

                case 2:
                    set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.UsersContent);
                    break;

                case 3:
                    set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.PodcastsContent);
                    break;
            }

            set.Apply();
            TableView.ReloadData();
        }
    }
}