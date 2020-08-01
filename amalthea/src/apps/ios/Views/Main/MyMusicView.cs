using Cirrious.FluentLayouts.Touch;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Controls;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.ViewModels.Main;

namespace SoundByte.App.iOS.Views.Main
{
    [MvxTabPresentation(WrapInNavigationController = true, TabIconName = "my-music", TabName = "My Music")]
    public class MyMusicView : MvxTableViewController<MyMusicViewModel>
    {
        private SoundByteSegmentedControl _segmentedControl;
        private ContentTableViewSource _source;
        private MvxUIRefreshControl _refreshControl;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "My Music";
            NavigationController.NavigationBar.PrefersLargeTitles = true;

            // Create refresh control
            _refreshControl = new MvxUIRefreshControl();
            RefreshControl = _refreshControl;

            // Create table source
            _source = new ContentTableViewSource(TableView);

            // Create a segmented control (split between likes, and playlists)
            _segmentedControl = new SoundByteSegmentedControl(new[] { "Likes", "Playlists" });
            _segmentedControl.IndexChanged += OnSegmentedControlIndexChanged;

            TableView.TableHeaderView = _segmentedControl;

            // Apply constraints
            var constraints = View.VerticalStackPanelConstraints(new Margins(18, 0, 18, 0, 0, 0), _segmentedControl);
            View.AddConstraints(constraints);

            // Set source
            TableView.Source = _source;

            // Trigger the first click
            OnSegmentedControlIndexChanged(this, 0);
        }

        private void OnSegmentedControlIndexChanged(object sender, int index)
        {
            var set = this.CreateBindingSet<MyMusicView, MyMusicViewModel>();
            set.Bind(_refreshControl).For(x => x.RefreshCommand).To(vm => vm.RefreshCommand);
            set.Bind(_refreshControl).For(x => x.IsRefreshing).To(vm => vm.IsLoading);

            if (index == 0)
            {
                set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.LikesContent);
            }
            else
            {
                set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.PlaylistsContent);
            }

            set.Apply();
            TableView.ReloadData();
        }
    }
}