#nullable enable

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Views.Navigation;
using SoundByte.App.Uwp.Views.Panes;
using SoundByte.App.Uwp.Views.Playback;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.Media.Playback;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

using muxc = Microsoft.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.Views.Shell
{
    /// <summary>
    ///     Contains logic for the desktop shell. See XboxShell for the Xbox UI.
    /// </summary>
    public sealed partial class DesktopShell : Page, IAppShell
    {
        #region Overridden Variables

        public Frame RootFrame => ShellFrameContent;

        public BackEventHandler OnBack { get; set; }

        #endregion Overridden Variables

        #region Public Variables

        /// <summary>
        ///     Application Settings
        /// </summary>
        public SettingsService Settings { get; } = SettingsService.Instance;

        #endregion Public Variables

        #region Private Variables

        // Path used for navigation
        private string? _path;

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomeView)),
            ("browse", typeof(BrowseView)),
            ("podcasts", typeof(PodcastsView)),
            ("my-music", typeof(MyMusicView)),
        };

        #endregion Private Variables

        #region Setup

        public DesktopShell(string? path)
        {
            // Initialize the XAML
            InitializeComponent();

            // Set the path
            _path = path;

            // Set the accent color
            TitlebarHelper.SetInitialStyle();

            // Change thickness for Xbox, Holographic and team
            Application.Current.Resources["TitleMargin"] = new Thickness(0, DeviceHelper.IsHolographic || DeviceHelper.IsTeam ? 12.0 : 24.0, 0, 0);

            // This is a dirty to show the now playing
            // bar when a track is played. This method
            // updates the required layout for the now
            // playing bar.
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange += InstanceOnOnCurrentTrackChanged;

            // No transitions on the side frame.
            SideFrame.ContentTransitions = null;
            SideFrame.Transitions = null;
        }

        #endregion Setup

        #region Shell Events

        private void ShellLoaded(object sender, RoutedEventArgs e) => ShellLoadedAsync().FireAndForgetSafeAsync();

        private async Task ShellLoadedAsync() => await PerformWorkAsync();

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        #endregion Shell Events

        #region Overridden Methods

        public void Dispose()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange -= InstanceOnOnCurrentTrackChanged;
        }

        public object GetName(string name) => FindName(name);

        public async Task PerformWorkAsync()
        {
            // Let the user know we are loading
            await App.SetLoadingAsync(true);

            // Load the application extensions
            try
            {
                await SimpleIoc.Default.GetInstance<IExtensionService>().FindAndLoadExtensionsAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Could not load extensions").ShowAsync();
            }

            // Set the actual title bar style
            TitlebarHelper.UpdateTitlebarStyle();

            // Set the application language
            ApplicationLanguages.PrimaryLanguageOverride =
                string.IsNullOrEmpty(SettingsService.Instance.CurrentAppLanguage)
                    ? ApplicationLanguages.Languages[0]
                    : SettingsService.Instance.CurrentAppLanguage;

            // Handle system back requests
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            // Navigate to the first page
            if (!await ProtocolHelper.HandleProtocolAsync(_path))
            {
                switch (SettingsService.Instance.StartPage)
                {
                    case "home":
                        RootFrame.Navigate(typeof(HomeView));
                        break;

                    case "browse":
                        RootFrame.Navigate(typeof(BrowseView));
                        break;

                    case "podcasts":
                        RootFrame.Navigate(typeof(PodcastsView));
                        break;

                    case "my-music":
                        RootFrame.Navigate(typeof(MyMusicView));
                        break;

                    default:
                        RootFrame.Navigate(typeof(HomeView));
                        break;
                }
            }

            // Hide the loading bar as the rest of the content is loaded in the background
            await App.SetLoadingAsync(false);

            //// Rate and Review dialog if we are not xbox, have the dialog enabled, and the launch count
            //// is greater than 10
            //if (Settings.EnableRating && !DeviceHelper.IsXbox && SystemInformation.LaunchCount > 10)
            //{
            //    var enjoyDialog = new MessageDialog("Enjoying SoundByte?");
            //    enjoyDialog.Commands.Add(new UICommand("Not really", a =>
            //    {
            //        // Show the feedback dialog
            //        OpenSidePane(typeof(FeedbackPaneView), "Feedback");

            //        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Not enjoy, leave feedback.");

            //        Settings.EnableRating = false;
            //    }));

            //    enjoyDialog.Commands.Add(new UICommand("Yes!", async a =>
            //    {
            //        var ratingDialog = new MessageDialog("How about leaving a rating on the store?");
            //        ratingDialog.Commands.Add(new UICommand("No thanks", _ =>
            //        {
            //            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, no review.");
            //        }));

            //        ratingDialog.Commands.Add(new UICommand("Okay", async _ =>
            //        {
            //            if (ApiInformation.IsMethodPresent("Windows.Services.Store.StoreContext", "RequestRateAndReviewAppAsync"))
            //            {
            //                var storeContext = StoreContext.GetDefault();
            //                var result = await storeContext.RequestRateAndReviewAppAsync();
            //                switch (result.Status)
            //                {
            //                    case StoreRateAndReviewStatus.Succeeded:
            //                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: Succeeded.");
            //                        Settings.EnableRating = false;
            //                        break;

            //                    case StoreRateAndReviewStatus.CanceledByUser:
            //                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: CanceledByUser.");
            //                        Settings.EnableRating = false;
            //                        break;

            //                    case StoreRateAndReviewStatus.NetworkError:
            //                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: NetworkError.");
            //                        Settings.EnableRating = true;
            //                        break;

            //                    case StoreRateAndReviewStatus.Error:
            //                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: Error.");
            //                        Settings.EnableRating = true;
            //                        break;
            //                }
            //            }
            //            else
            //            {
            //                Settings.EnableRating = false;
            //                await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
            //            }
            //        }));

            //        await ratingDialog.ShowAsync();
            //    }));

            //    enjoyDialog.DefaultCommandIndex = 1;
            //    enjoyDialog.CancelCommandIndex = 0;

            //    await enjoyDialog.ShowAsync();
            //}

            // Show tips
            //ExtensionTeachingTip.IsOpen = true;
            //BetaVersionTip.IsOpen = true;

            // Run on the background thread
            await Task.Run(async () =>
            {
                try
                {
                    // Continue playlist last song (if not already playing something)
                    var status = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaybackState();
                    if (status == MediaPlaybackState.None)
                    {
                        await ProtocolHelper.HandleResumeAsync();
                        SimpleIoc.Default.GetInstance<IPlaybackService>().PauseTrack();
                    }
                }
                catch
                {
                    // Not worried
                }

                // Clear the unread badge
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear();

                // Only run this code if the store is supported. Get a list of pending updates and show
                // the download button only if there are updates.
                if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
                {
                    // Get the updates and then show/hide the download button
                    var pendingUpdates = await StoreContext.GetDefault().GetAppAndOptionalStorePackageUpdatesAsync();
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() => UpdatePendingButton.Visibility = pendingUpdates?.Count > 0 ? Visibility.Visible : Visibility.Collapsed);
                }

                // Load logged in user objects
                var result = await SoundByteService.Current.InitUsersAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    await NavigationService.Current.CallMessageDialogAsync(result, "Account Error");
                }

                // Attempt to start the background connection
                //await BackendService.Instance.TryConnectAsync();

                // Xbox does not support cortana. No point loading the voice commands
                if (!DeviceHelper.IsXbox)
                {
                    try
                    {
                        // Install Cortana Voice Commands
                        var vcdStorageFile = await Package.Current.InstalledLocation.GetFileAsync(@"SoundByteCommands.xml");
                        await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(vcdStorageFile);
                    }
                    catch (Exception ex)
                    {
                        SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
                    }
                }
            });
        }

        #endregion Overridden Methods

        #region General Events

        /// <summary>
        ///     This code is run when the playing track changes. Used to show
        ///     the now playing bar.
        /// </summary>
        /// <param name="newTrack">The new track to play</param>
        private async void InstanceOnOnCurrentTrackChanged(BaseTrack newTrack)
        {
            // Run on the UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Update the application title
                AppTitle.Text = "SoundByte - " + newTrack.Title;

                // Show the now playing bar (if hidden)
                await ShowNowPlayingBarAsync();
            });
        }

        #endregion General Events

        #region Back Events

        /// <summary>
        ///     Called when a back event is sent though the system.
        /// </summary>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            HandleGoBack();
            e.Handled = true;
        }

        /// <summary>
        ///     Handle back events in the application
        /// </summary>
        private void HandleGoBack()
        {
            // Call the back event if it has been subscribed to
            if (OnBack != null)
            {
                OnBack.Invoke();
                return;
            }

            // If we can go back, go back
            if (RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
                return;
            }

            // By default navigate to the home view
            RootFrame.Navigate(typeof(HomeView));
        }

        #endregion Back Events

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

        #region Navigation Events

        /// <summary>
        ///     Called when the side frame navigates
        /// </summary>
        private void SideFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Change the back button visibility
            PaneBackButton.Visibility = SideFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        ///     Called when the shell frame navigates
        /// </summary>
        private void ShellFrame_Navigated(object sender, NavigationEventArgs e) => ShellFrame_NavigatedAsync(sender, e).FireAndForgetSafeAsync();

        private async Task ShellFrame_NavigatedAsync(object sender, NavigationEventArgs e)
        {
            // Update the back button status
            NavigationView.IsBackEnabled = ShellFrameContent.CanGoBack;

            var item = _pages.Find(p => p.Page == e.SourcePageType);
            if (item.Page == null)
            {
                // None of the main screens are selected, so selected nothing
                NavigationView.SelectedItem = null;
            }
            else
            {
                // Select the current item
                NavigationView.SelectedItem = NavigationView.MenuItems
                    .OfType<muxc.NavigationViewItem>().First(x => x.Tag.Equals(item.Tag));
            }

            // Update the now playing bar.
            if (SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack() == null)
                await HideNowPlayingBarAsync();
            else
                await ShowNowPlayingBarAsync();
        }

        /// <summary>
        ///     When the back button is pressed.
        /// </summary>
        private void NavigationView_BackRequested(muxc.NavigationView sender, muxc.NavigationViewBackRequestedEventArgs args)
            => HandleGoBack();

        /// <summary>
        ///     This method is called when the user clicks on a top level navigation
        ///     element.
        /// </summary>
        private void NavigationView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            // Get the page from the tag.
            var navItemTag = args.InvokedItemContainer?.Tag?.ToString();
            var page = _pages.Find(p => p.Tag.Equals(navItemTag)).Page;

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ShellFrameContent.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(page is null) && !Type.Equals(preNavPageType, page))
            {
                // Navigate and don't store in back stack
                ShellFrameContent.Navigate(page, null, args.RecommendedNavigationTransitionInfo);
            }
        }

        /// <summary>
        ///     Called when the user clicks on the small buttons on the upper right.
        ///     Most of these actions open the smaller window pane.
        /// </summary>
        private async void NavigateCommandBar(object sender, RoutedEventArgs e)
        {
            // Switch on the button tag
            switch (((FrameworkElement)sender).Tag)
            {
                case "music-providers":
                    OpenSidePane(typeof(MusicProvidersPaneView), "Music Providers");
                    break;

                case "account":
                    await NavigationService.Current.CallDialogAsync<ManageMusicProvidersDialog>();
                    break;

                case "playing":
                    RootFrame.Navigate(typeof(NowPlayingView));
                    break;

                case "settings":
                    OpenSidePane(typeof(SettingsPaneView), "Settings");
                    break;

                case "about":
                    await NavigationService.Current.CallDialogAsync<AboutDialog>();
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

                case "update":
                    try
                    {
                        // Get a list of updates and try download them in the background
                        if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
                        {
                            var pendingUpdates = await StoreContext.GetDefault().GetAppAndOptionalStorePackageUpdatesAsync();
                            await StoreContext.GetDefault().RequestDownloadAndInstallStorePackageUpdatesAsync(pendingUpdates);
                        }
                    }
                    catch
                    {
                        await NavigationService.Current.CallMessageDialogAsync("An error occurred while trying to update SoundByte. Please try again later.", "Update Failed");
                    }

                    break;

                case "help":
                    await Launcher.LaunchUriAsync(new Uri("https://soundbytemedia.com/pages/faq"));
                    break;

                case "feedback":
                    await Launcher.LaunchUriAsync(new Uri("https://github.com/DominicMaas/SoundByteOSS/issues"));
                    break;

                case "new":
                    await Launcher.LaunchUriAsync(new Uri($"https://github.com/DominicMaas/SoundByteOSS/wiki/Changelog-(UWP)"));
                    break;

                case "review":
                    if (ApiInformation.IsMethodPresent("Windows.Services.Store.StoreContext", "RequestRateAndReviewAppAsync"))
                    {
                        var storeContext = StoreContext.GetDefault();
                        var result = await storeContext.RequestRateAndReviewAppAsync();
                        switch (result.Status)
                        {
                            case StoreRateAndReviewStatus.Succeeded:
                                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: Succeeded.");
                                Settings.EnableRating = false;
                                break;

                            case StoreRateAndReviewStatus.CanceledByUser:
                                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: CanceledByUser.");
                                Settings.EnableRating = false;
                                break;

                            case StoreRateAndReviewStatus.NetworkError:
                                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: NetworkError.");
                                Settings.EnableRating = true;
                                break;

                            case StoreRateAndReviewStatus.Error:
                                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Rating: Enjoy, review: Error.");
                                Settings.EnableRating = true;
                                break;
                        }
                    }
                    else
                    {
                        Settings.EnableRating = false;
                        await Launcher.LaunchUriAsync(new Uri($"ms-windows-store:REVIEW?PFN={Package.Current.Id.FamilyName}"));
                    }

                    break;
            }
        }

        #endregion Navigation Events

        #region Public Methods

        /// <summary>
        ///     Close the side pane
        /// </summary>
        public void CloseSidePane() => PageSplitView.IsPaneOpen = false;

        /// <summary>
        ///     Open the side pane
        /// </summary>
        /// <param name="pageType">The page to open</param>
        /// <param name="title">The title to display</param>
        public void OpenSidePane(Type pageType, string title)
        {
            SideFrame.Navigate(pageType, null, new SuppressNavigationTransitionInfo());
            SideFrame.BackStack.Clear();
            PaneTitle.Text = title;
            PaneBackButton.Visibility = Visibility.Collapsed;
            PageSplitView.IsPaneOpen = true;
        }

        #endregion Public Methods

        #region Search Events

        /// <summary>
        ///     Called when the user searches for something.
        /// </summary>
        private void SearchForItem(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            // If the user has not typed anything, don't search
            if (string.IsNullOrEmpty(args.QueryText))
                return;

            // Navigate to search view
            App.NavigateTo(typeof(SearchView), args.QueryText);
        }

        private class Track
        {
            public string name { get; set; }
            public string artist { get; set; }
            public string url { get; set; }
            public string streamable { get; set; }
            public string listeners { get; set; }
            public string mbid { get; set; }
        }

        private class Trackmatches
        {
            public List<Track> track { get; set; }
        }

        private class SearchResults
        {
            public Trackmatches trackmatches { get; set; }
        }

        private class SearchQuery
        {
            public SearchResults Results { get; set; }
        }

        /// <summary>
        ///     When the user types in some text, need to search the last.fm API to get results.
        /// </summary>
        private async void SearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Clear if nothing is typed in
            if (string.IsNullOrEmpty(sender.Text))
            {
                sender.ItemsSource = null;
                return;
            }

            try
            {
                // Get items
                var results = await SoundByteService.Current.GetAsync<SearchQuery>(
                    $"http://ws.audioscrobbler.com/2.0/?method=track.search&track={sender.Text}&api_key=f21aacb1094e4abb36c4b11b618bccbe&format=json");

                // Add items to UI
                var suggestionList = new List<string>();
                foreach (var track in results.Response.Results.trackmatches.track)
                {
                    suggestionList.Add(track.name);
                }

                sender.ItemsSource = suggestionList;
            }
            catch
            {
                sender.ItemsSource = null;
            }
        }

        /// <summary>
        ///     When the user chooses something from the auto suggest list.
        /// </summary>
        private void SearchBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // If the user has not typed anything, don't search
            if (string.IsNullOrEmpty(args.SelectedItem.ToString()))
                return;

            // Navigate to search view
            App.NavigateTo(typeof(SearchView), args.SelectedItem.ToString());
        }

        #endregion Search Events
    }
}

#nullable restore