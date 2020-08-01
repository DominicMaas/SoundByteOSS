#nullable enable

using System;
using Cirrious.FluentLayouts.Touch;
using MvvmCross;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Navigation;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Platforms.Ios.Views;
using SoundByte.App.iOS.Controls;
using SoundByte.App.iOS.Sources;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Services.Implementations;
using SoundByte.Core.ViewModels;
using SoundByte.Core.ViewModels.Main;
using UIKit;

namespace SoundByte.App.iOS.Views.Main
{
    [MvxTabPresentation(WrapInNavigationController = true, TabIconName = "discover", TabName = "Discover")]
    public class DiscoverView : MvxTableViewController<DiscoverViewModel>
    {
        private SoundByteSegmentedControl? _segmentedControl;
        private ContentTableViewSource _source;
        private MvxUIRefreshControl _refreshControl;
        private UISearchController _searchController;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Discover";
            NavigationController.NavigationBar.PrefersLargeTitles = true;

            // Grab the settings service
            var settingsService = Mvx.IoCProvider.Resolve<ISettingsService>();

            // Get the app mode (by default the app mode is both)
            var appMode = settingsService.GetPreference(SettingsService.AppMode, AppMode.Both);

            // Create a search controller
            _searchController = new UISearchController(searchResultsController: null)
            {
                DimsBackgroundDuringPresentation = true,
            };

            // Handle the click event
            _searchController.SearchBar.SearchButtonClicked += SearchBar_SearchButtonClicked;

            // Set the search controller
            NavigationItem.SearchController = _searchController;
            NavigationItem.HidesSearchBarWhenScrolling = false;

            // Create refresh control
            _refreshControl = new MvxUIRefreshControl();
            RefreshControl = _refreshControl;

            // Create table source
            _source = new ContentTableViewSource(TableView);

            // Create a segmented control (split between media, and podcasts) - only if app mode is both
            if (appMode == AppMode.Both)
            {
                _segmentedControl = new SoundByteSegmentedControl(new[] { "Media", "Podcasts" });
                _segmentedControl.IndexChanged += OnSegmentedControlIndexChanged;

                // Set on the table view
                TableView.TableHeaderView = _segmentedControl;
            }

            // Set source
            TableView.Source = _source;

            // Set the initial view
            switch (appMode)
            {
                case AppMode.Both:
                case AppMode.Media:
                    ChangeSource(0);
                    break;

                case AppMode.Podcast:
                    ChangeSource(1);
                    break;
            }
        }

        private void SearchBar_SearchButtonClicked(object sender, EventArgs e)
        {
            var navigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
            navigationService.Navigate<SearchViewModel, string>(_searchController.SearchBar.Text);
        }

        private void OnSegmentedControlIndexChanged(object sender, int index)
        {
            ChangeSource(index);
        }

        private void ChangeSource(int index)
        {
            var set = this.CreateBindingSet<DiscoverView, DiscoverViewModel>();
            set.Bind(_refreshControl).For(x => x.RefreshCommand).To(vm => vm.RefreshCommand);
            set.Bind(_refreshControl).For(x => x.IsRefreshing).To(vm => vm.IsLoading);

            if (index == 0)
            {
                set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.MediaContent);
            }
            else
            {
                set.Bind(_source).For(x => x.ItemsSource).To(vm => vm.PodcastsContent);
            }

            set.Apply();
            TableView.ReloadData();
        }
    }
}