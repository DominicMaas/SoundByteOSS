#nullable enable

using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core.Services;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Views.Xbox;
using System;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Globalization;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

namespace SoundByte.App.Uwp.Views.Shell
{
    /// <summary>
    ///     Contains logic for the xbox shell. See DesktopShell for the desktop UI.
    /// </summary>
    public sealed partial class XboxShell : Page, IAppShell
    {
        #region Overridden Variables

        public Frame RootFrame => ShellFrameContent;

        public BackEventHandler OnBack { get; set; }

        #endregion Overridden Variables

        #region Private Variables

        // Is the navigation blocked
        private bool _isNavigationBlocked;

        // Index for the current navigation tab, used for animations
        private int _navigationIndex;

        // Path used for navigation
        private readonly string _path;

        #endregion Private Variables

        #region Constructor

        /// <summary>
        ///     Load the xbox shell. Very minimal code is run to ensure fast startup time.
        /// </summary>
        /// <param name="path"></param>
        public XboxShell(string path)
        {
            // Initialize the XAML
            InitializeComponent();

            _path = path;
        }

        #endregion Constructor

        #region Shell Events

        private void ShellLoaded(object sender, RoutedEventArgs e) => ShellLoadedAsync().FireAndForgetSafeAsync();

        private async Task ShellLoadedAsync() => await PerformWorkAsync();

        #endregion Shell Events

        #region Overridden Methods

        public void Dispose()
        { }

        public object GetName(string name) => FindName(name);

        public async Task PerformWorkAsync()
        {
            // Set the application language
            ApplicationLanguages.PrimaryLanguageOverride =
                string.IsNullOrEmpty(SettingsService.Instance.CurrentAppLanguage)
                    ? ApplicationLanguages.Languages[0]
                    : SettingsService.Instance.CurrentAppLanguage;

            // Load the application extensions
            try
            {
                await SimpleIoc.Default.GetInstance<IExtensionService>().FindAndLoadExtensionsAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message, "Could not load extensions").ShowAsync();
            }

            // Handle system back requests
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            // Navigate to the Xbox explore view
            RootFrame.Navigate(typeof(XboxExploreView));

            // Run on the background thread
            await Task.Run(async () =>
            {
                try
                {
                    // Load premium
                    if (ApiInformation.IsTypePresent("Windows.Services.Store.StoreContext"))
                        await PremiumService.Current.InitProductInfoAsync();
                }
                catch (Exception ex)
                {
                    await NavigationService.Current.CallMessageDialogAsync(ex.Message, "Error loading premium status information.");
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
                }

                // Load logged in user objects
                var result = await SoundByteService.Current.InitUsersAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    await NavigationService.Current.CallMessageDialogAsync(result, "Account Error");
                }
            });
        }

        #endregion Overridden Methods

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

            // By default navigate to the xbox explore view
            RootFrame.Navigate(typeof(XboxExploreView));
        }

        #endregion Back Events

        #region Navigation Events

        private void ShellFrameNavigated(object sender, NavigationEventArgs e)
        {
            // Block any navigation
            _isNavigationBlocked = true;

            // Update the top navigation bar
            switch (((Frame)sender).SourcePageType.Name)
            {
                case nameof(XboxPlayingView):
                    _navigationIndex = 1;
                    PlayingTab.IsChecked = true;
                    RootFrame.Margin = new Thickness { Top = 0 };
                    break;

                case nameof(XboxExploreView):
                    _navigationIndex = 2;
                    ExploreTab.IsChecked = true;
                    RootFrame.Margin = new Thickness { Top = 0 };
                    break;

                case nameof(XboxMusicView):
                    _navigationIndex = 3;
                    MusicTab.IsChecked = true;
                    RootFrame.Margin = new Thickness { Top = 0 };
                    break;

                case nameof(XboxSearchView):
                    _navigationIndex = 4;
                    SearchTab.IsChecked = true;
                    RootFrame.Margin = new Thickness { Top = 0 };
                    break;

                case nameof(XboxSettingsView):
                    _navigationIndex = 5;
                    SettingsTab.IsChecked = true;
                    RootFrame.Margin = new Thickness { Top = 0 };
                    break;

                default:
                    _navigationIndex = 0;
                    UnknownTab.IsChecked = true;
                    RootFrame.Margin = new Thickness { Top = 80 };
                    break;
            }

            // Unblock navigation
            _isNavigationBlocked = false;
        }

        private void NavTabClicked(object sender, RoutedEventArgs e)
        {
            // Don't perform navigation when blocked (when setting the boxes to checked).
            if (_isNavigationBlocked)
                return;

            // Navigation effects (side navigation only supported on v7+)
            var navTransitionRight = new SlideNavigationTransitionInfo();
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
                navTransitionRight.Effect = SlideNavigationTransitionEffect.FromRight;

            var navTransitionLeft = new SlideNavigationTransitionInfo();
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
                navTransitionLeft.Effect = SlideNavigationTransitionEffect.FromLeft;

            // Switch on the selected item name
            var name = ((RadioButton)sender).Name;
            switch (name)
            {
                case nameof(PlayingTab):
                    if (RootFrame.CurrentSourcePageType != typeof(XboxPlayingView))
                        RootFrame.Navigate(typeof(XboxPlayingView), null, _navigationIndex > 1 ? navTransitionLeft : navTransitionRight);
                    break;

                case nameof(ExploreTab):
                    if (RootFrame.CurrentSourcePageType != typeof(XboxExploreView))
                        RootFrame.Navigate(typeof(XboxExploreView), null, _navigationIndex > 2 ? navTransitionLeft : navTransitionRight);
                    break;

                case nameof(MusicTab):
                    if (RootFrame.CurrentSourcePageType != typeof(XboxMusicView))
                        RootFrame.Navigate(typeof(XboxMusicView), null, _navigationIndex > 3 ? navTransitionLeft : navTransitionRight);
                    break;

                case nameof(SearchTab):
                    if (RootFrame.CurrentSourcePageType != typeof(XboxSearchView))
                        RootFrame.Navigate(typeof(XboxSearchView), null, _navigationIndex > 4 ? navTransitionLeft : navTransitionRight);
                    break;

                case nameof(SettingsTab):
                    if (RootFrame.CurrentSourcePageType != typeof(XboxSettingsView))
                        RootFrame.Navigate(typeof(XboxSettingsView), null, _navigationIndex > 5 ? navTransitionLeft : navTransitionRight);
                    break;
            }
        }

        #endregion Navigation Events
    }
}

#nullable restore