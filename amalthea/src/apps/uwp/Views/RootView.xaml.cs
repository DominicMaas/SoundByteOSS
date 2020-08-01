using MvvmCross;
using MvvmCross.Platforms.Uap.Views;
using MvvmCross.ViewModels;
using SoundByte.App.UWP.Dialogs;
using SoundByte.App.UWP.Extensions;
using SoundByte.App.UWP.Presenters;
using SoundByte.Core.ViewModels;
using SoundByte.Core.ViewModels.Main;
using SoundByte.Core.ViewModels.Panes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.Core.Models.Media;
using muxc = Microsoft.UI.Xaml.Controls;

namespace SoundByte.App.UWP.Views
{
    [MvxViewFor(typeof(RootViewModel))]
    public sealed partial class RootView : MvxWindowsPage, ISoundByteHost
    {
        #region Private Variables

        private bool _isPresentedFirstTime = true;

        #endregion Private Variables

        public RootViewModel Vm => (RootViewModel)ViewModel;

        public MeViewModel MeVm { get; set; }

        public RootView() => InitializeComponent();

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (Vm != null && _isPresentedFirstTime)
            {
                _isPresentedFirstTime = false;
                await Vm.NavigateCommand.ExecuteAsync(typeof(HomeViewModel));

                // Set the titlebar
                var textColor = ActualTheme == ElementTheme.Dark
                    ? Colors.White
                    : Colors.Black;

                // Extend UI
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

                // Set custom titlebar
                AppTitle.Foreground = new SolidColorBrush(textColor);
                Window.Current.SetTitleBar(Titlebar);

                // Update Title bar colors
                ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor =
                    new Color { R = 0, G = 0, B = 0, A = 20 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor =
                    new Color { R = 0, G = 0, B = 0, A = 60 };
                ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = textColor;
                ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = textColor;

                // Set up the "Me" View model, on mobile apps / xbox this is a seperate page, but
                // on PC, this is included in the root view.

                // Load the viewModel
                var viewModelLoader = Mvx.IoCProvider.Resolve<IMvxViewModelLoader>();
                MeVm = (MeViewModel)viewModelLoader.LoadViewModel(new MvxViewModelRequest<MeViewModel>(), null);

                Vm.GetPlaybackService().OnMediaChange += RootView_OnMediaChange;
            }
        }

        private async void RootView_OnMediaChange(Media media)
        {
            // Run on the UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Update the application title
                if (media is Track t)
                {
                    AppTitle.Text = "SoundByte - " + t.Title;
                }
                else
                {
                    AppTitle.Text = "SoundByte - Music Player";
                }

                // Show the now playing bar (if hidden)
                await ShowNowPlayingBarAsync();
            });
        }

        public void CloseModal()
        {
            SideFrame.Content = null;
            SideFrame.BackStack.Clear();
            PageSplitView.IsPaneOpen = false;
        }

        public void ShowPage(Type pageType, object parameter, string tag, NavigationTransitionInfo navigationTransitionInfo)
        {
            // Set the shell content
            ShellFrameContent.Navigate(pageType, parameter, navigationTransitionInfo);

            // Set the selected item on the UI
            if (!string.IsNullOrEmpty(tag))
            {
                NavigationView.SelectedItem = NavigationView.MenuItems
                    .OfType<muxc.NavigationViewItem>().First(x => x.Tag.Equals(tag));
            }
        }

        public async Task NavigateBackAsync()
        {
            // If we can go back, go back
            if (ShellFrameContent.CanGoBack)
            {
                ShellFrameContent.GoBack();

                var currentViewType = ShellFrameContent.SourcePageType;

                // Ensure the highlighting is correct
                UpdateSelectedUi(currentViewType);
            }
            else
            {
                // By default navigate to the home view
                await Vm.NavigateCommand.ExecuteAsync(typeof(HomeViewModel));

                // Ensure the correct item is selected
                NavigationView.SelectedItem = NavigationView.MenuItems
                    .OfType<muxc.NavigationViewItem>().First(x => x.Tag.Equals(typeof(HomeViewModel).GetTabAttribute().Tag));
            }
        }

        private void UpdateSelectedUi(Type t)
        {
            // Get the tag if a tab (ensure the correct tab on the top of the page is selected)
            string tag = null;
            if (t.HasTabAttribute())
                tag = t.GetTabAttribute().Tag;

            // Ensure the correct item is selected
            if (!string.IsNullOrEmpty(tag))
            {
                NavigationView.SelectedItem = NavigationView.MenuItems
                    .OfType<muxc.NavigationViewItem>().First(x => x.Tag.Equals(tag));
            }
            else
            {
                NavigationView.SelectedItem = null;
            }
        }

        public void ShowModal(Type pageType, object parameter, string title)
        {
            SideFrame.Navigate(pageType, parameter, new SuppressNavigationTransitionInfo());
            SideFrame.BackStack.Clear();
            PaneTitle.Text = title;
            PaneBackButton.Visibility = Visibility.Collapsed;
            PageSplitView.IsPaneOpen = true;
        }

        private async void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            await NavigateBackAsync();
        }

        private async void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            switch (args.InvokedItemContainer.Tag.ToString())
            {
                case "home":
                    await Vm.NavigateCommand.ExecuteAsync(typeof(HomeViewModel));
                    break;

                case "discover":
                    await Vm.NavigateCommand.ExecuteAsync(typeof(DiscoverViewModel));
                    break;

                case "podcasts":
                    await Vm.NavigateCommand.ExecuteAsync(typeof(PodcastsViewModel));
                    break;

                case "my-music":
                    await Vm.NavigateCommand.ExecuteAsync(typeof(MyMusicViewModel));
                    break;
            }
        }

        private async void ShellFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Update the back button status
            NavigationView.IsBackEnabled = ShellFrameContent.CanGoBack;

            // Ensure the highlighting is correct
            UpdateSelectedUi(e.SourcePageType);

            // Update the now playing bar.
            if (Vm.GetPlaybackService().GetCurrentMedia() == null)
                await HideNowPlayingBarAsync();
            else
                await ShowNowPlayingBarAsync();
        }

        private void SideFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Change the back button visibility
            PaneBackButton.Visibility = SideFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void NavigateCommandBar(object sender, RoutedEventArgs e)
        {
            switch (((FrameworkElement)sender).Tag.ToString())
            {
                case "music-provider-load":
                    await LoadExtensionAsync();
                    break;

                case "close-pane":
                    PageSplitView.IsPaneOpen = false;
                    break;

                case "back-pane":
                    if (SideFrame.CanGoBack)
                    {
                        SideFrame.GoBack();
                    }
                    else
                    {
                        PageSplitView.IsPaneOpen = false;
                    }
                    break;

                case "about":
                    await Vm.GetDialogService().ShowDialogAsync<AboutDialog>();
                    break;
            }
        }

        private async void SearchBox_OnSuggestionChosen(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // If the user has not typed anything, don't search
            if (string.IsNullOrEmpty(args.SelectedItem.ToString()))
                return;

            // Navigate to search view
            await Vm.GetNavigationService().Navigate<SearchViewModel, string>(args.SelectedItem.ToString());
        }

        private void SearchBox_OnTextChanged(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            // TODO
        }

        private async void SearchForItem(Windows.UI.Xaml.Controls.AutoSuggestBox sender, Windows.UI.Xaml.Controls.AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // If the user has not typed anything, don't search
            if (string.IsNullOrEmpty(args.QueryText))
                return;

            // Navigate to search view
            await Vm.GetNavigationService().Navigate<SearchViewModel, string>(args.QueryText);
        }

        #region Now Playing Bar Events

        /// <summary>
        ///     Animate out and hide the now playing bar.
        /// </summary>
        private async Task HideNowPlayingBarAsync()
        {
            // If already hidden, don't hide
            if (!(FindName("NowPlaying") is DropShadowPanel nowPlayingBar)
                || nowPlayingBar.Tag?.ToString() == "hide")
                return;

            // Hide the now playing bar
            await nowPlayingBar.Offset(0, 0, 150, 0, EasingType.Cubic).Fade(0.5f, 150, 0, EasingType.Cubic).StartAsync();
            nowPlayingBar.Tag = "hide";
        }

        /// <summary>
        ///     Animate in and show the now playing bar.
        /// </summary>
        private async Task ShowNowPlayingBarAsync()
        {
            // If already shown, don't show
            if (!(FindName("NowPlaying") is DropShadowPanel nowPlayingBar)
                || nowPlayingBar.Tag?.ToString() == "show")
                return;

            // Show the now playing bar
            await nowPlayingBar.Offset(0, -200, 300, 0, EasingType.Cubic).Fade(1.0f, 300, 0, EasingType.Cubic).StartAsync();
            nowPlayingBar.Tag = "show";
        }

        #endregion Now Playing Bar Events

        #region MeViewModel Helper Methods

        private async Task LoadExtensionAsync()
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Get the temp folder
                var cache = ApplicationData.Current.LocalCacheFolder;
                var tempFolder = await cache.CreateFolderAsync("tmp", CreationCollisionOption.OpenIfExists);
                var extensionsFolder = await tempFolder.CreateFolderAsync("extensions", CreationCollisionOption.OpenIfExists);
                var extensionFolder = await extensionsFolder.CreateFolderAsync(Guid.NewGuid().ToString());

                // Copy the folder
                await CopyFolderAsync(folder, extensionFolder);

                // Load the music provider from this new folder
                await MeVm.LoadMusicProviderCommand.ExecuteAsync(Path.Combine(extensionFolder.Path, folder.Name));
            }
        }

        public static async Task CopyFolderAsync(StorageFolder source, StorageFolder destinationContainer, string desiredName = null)
        {
            StorageFolder destinationFolder = null;
            destinationFolder = await destinationContainer.CreateFolderAsync(
                desiredName ?? source.Name, CreationCollisionOption.ReplaceExisting);

            foreach (var file in await source.GetFilesAsync())
            {
                await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.ReplaceExisting);
            }
            foreach (var folder in await source.GetFoldersAsync())
            {
                await CopyFolderAsync(folder, destinationFolder);
            }
        }

        #endregion MeViewModel Helper Methods
    }
}