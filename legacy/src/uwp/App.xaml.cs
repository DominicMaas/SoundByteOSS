#nullable enable

using GalaSoft.MvvmLight.Ioc;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels;
using SoundByte.App.Uwp.Views;
using SoundByte.App.Uwp.Views.Playback;
using SoundByte.App.Uwp.Views.Shell;
using SoundByte.Core;
using SoundByte.Core.Items;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Managers;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SoundByte.App.Uwp
{
    sealed partial class App
    {
        /// <summary>
        ///     Used for roaming content across devices and platforms.
        /// </summary>
        public static RoamingService RoamingService { get; } = new RoamingService();

        public static SourceManager SourceManager { get; } = new SourceManager();

        private bool _isInit;

        private bool _isCtrlKeyPressed;

        #region App Setup

        /// <summary>
        ///     This is the main class for this app. This function is the first function
        ///     called and it setups the app analytic (If in release mode), components,
        ///     requested theme and event handlers.
        /// </summary>
        public App()
        {
            // Initialize XAML Resources
            InitializeComponent();

            // Check that we are not using the default theme,
            // if not change the requested theme to the users
            // picked theme.
            if (!SettingsService.Instance.IsDefaultTheme)
                RequestedTheme = SettingsService.Instance.ThemeType;

            // We want to use the controller if on xbox
            if (DeviceHelper.IsXbox)
            {
                RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            }

            try
            {
                // Handle application crashes
                CrashHelper.HandleAppCrashes(this);
            }
            catch { }

            // Register Event Handlers
            EnteredBackground += AppEnteredBackground;
            LeavingBackground += AppLeavingBackground;
            Suspending += AppSuspending;

            // Used Reveal Focus on 1803+
            if (ApiInformation.IsEnumNamedValuePresent("Windows.UI.Xaml.FocusVisualKind", nameof(FocusVisualKind.Reveal)))
                FocusVisualKind = FocusVisualKind.Reveal;

            // During the transition from foreground to background the
            // memory limit allowed for the application changes. The application
            // has a short time to respond by bringing its memory usage
            // under the new limit.
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;

            // After an application is backgrounded it is expected to stay
            // under a memory target to maintain priority to keep running.
            // Subscribe to the event that informs the app of this change.
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;

            // Run this code when a service is connected to SoundByte
            SoundByteService.Current.OnServiceConnected += (type, token) =>
            {
                var vault = new PasswordVault();

                // Add the password to the vault so we can access it when restarting the app
                string vaultName;
                switch (type)
                {
                    case ServiceTypes.SoundCloud:
                    case ServiceTypes.SoundCloudV2:
                        vaultName = "SoundByte.SoundCloud";
                        break;

                    case ServiceTypes.YouTube:
                        vaultName = "SoundByte.YouTube";
                        break;

                    default:
                        vaultName = string.Empty;
                        break;
                }

                if (string.IsNullOrEmpty(vaultName))
                    return;

                vault.Add(new PasswordCredential(vaultName, "Token", token.AccessToken));
                vault.Add(new PasswordCredential(vaultName, "RefreshToken", string.IsNullOrEmpty(token.RefreshToken) ? "n/a" : token.RefreshToken));
                vault.Add(new PasswordCredential(vaultName, "ExpireTime", string.IsNullOrEmpty(token.ExpireTime) ? "n/a" : token.ExpireTime));

                // Track the connect event
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Service Connected",
                    new Dictionary<string, string>
                    {
                        {"Service", type.ToString()}
                    });
            };

            // Run this code when a service is disconnected from SoundByte
            SoundByteService.Current.OnServiceDisconnected += async (type, reason) =>
            {
                // Delete the vault depending on the service type
                switch (type)
                {
                    case ServiceTypes.SoundCloud:
                    case ServiceTypes.SoundCloudV2:
                        SettingsService.Instance.DeleteAllFromVault("SoundByte.SoundCloud");
                        break;

                    case ServiceTypes.YouTube:
                        SettingsService.Instance.DeleteAllFromVault("SoundByte.YouTube");
                        break;
                }

                // Track the disconnect event
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Service Disconnected",
                    new Dictionary<string, string>
                    {
                        {"Service", type.ToString()},
                        {"Reason", reason }
                    });

                // Attempt to log the user back in
                if (!string.IsNullOrEmpty(reason))
                {
                    // Navigate to the login page
                    NavigateTo(typeof(AccountView), new AccountView.AccountViewParams
                    {
                        Service = type,
                    });

                    // Tell the user what happened
                    await NavigationService.Current.CallMessageDialogAsync("One of your accounts has been logged out. SoundByte will now redirect you to the login page.", "Account disconnected");
                }
            };
        }

        private void AppSuspending(object sender, SuspendingEventArgs e)
        {
            // Don't run on Xbox
            if (DeviceHelper.IsXbox)
                return;

            // Run on all other platforms
            AppSuspendingAsync(sender, e).FireAndForgetSafeAsync();
        }

        private async Task AppSuspendingAsync(object sender, SuspendingEventArgs e)
        {
            var def = e.SuspendingOperation.GetDeferral();

            try
            {
                // Clear live tile
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Clear();

                var playbackService = SimpleIoc.Default.GetInstance<IPlaybackService>();
                if (playbackService != null)
                {
                    // Save current position
                    var currentPosition = playbackService.GetMediaPlayer()?.PlaybackSession?.Position;
                    await RoamingService.StopActivityAsync(currentPosition);

                    // Update the resume files
                    var roamingFolder = ApplicationData.Current.RoamingFolder;

                    var track = new BaseSoundByteItem(playbackService.GetCurrentTrack());
                    var playlist = playbackService.GetMediaPlaybackList().Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack()));
                    var token = playbackService.GetPlaylistToken();
                    var source = playbackService.GetPlaylistSource();

                    var playbackFile = await roamingFolder.CreateFileAsync("currentPlayback.txt", CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(playbackFile,
                        ProtocolHelper.EncodeTrackProtocolItem(new ProtocolHelper.TrackProtocolItem(source, track, playlist, token, SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition(), SimpleIoc.Default.GetInstance<IPlaybackService>().IsPlaylistShuffled()), false) + "\n" + SettingsService.Instance.SessionId);
                }
            }
            catch (Exception ex)
            {
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackException(ex);
            }
            finally
            {
                def.Complete();
            }
        }

        private LoginToken? GetLoginTokenFromVault(string vaultName, int service)
        {
            // Get the password vault
            var vault = new PasswordVault();

            LoginToken loginToken;

            try
            {
                loginToken = new LoginToken
                {
                    AccessToken = vault.Retrieve(vaultName, "Token")?.Password,
                    ServiceType = service
                };
            }
            catch
            {
                return null;
            }

            try
            {
                loginToken.RefreshToken = vault.Retrieve(vaultName, "RefreshToken")?.Password;
                loginToken.ExpireTime = vault.Retrieve(vaultName, "ExpireTime")?.Password;
            }
            catch
            {
                // Ignore. In version 17.10, refresh and expire times were not used,
                // so the above will cause an exception when updating to the latest version.
                // Normally the crash would indicate that the user is not logged in, but in fact
                // they are. So we just ignore this.
            }

            return loginToken;
        }

        private void InitV3Service()
        {
            var soundCloudToken = GetLoginTokenFromVault("SoundByte.SoundCloud", ServiceTypes.SoundCloud);
            var youTubeToken = GetLoginTokenFromVault("SoundByte.YouTube", ServiceTypes.YouTube);

            var secretList = new List<ServiceInfo>
            {
                new ServiceInfo
                {
                    Service = ServiceTypes.SoundCloud,
                    ClientIds = AppKeys.SoundCloudClientIds,
                    ClientId = AppKeys.SoundCloudClientId,
                    UserToken = soundCloudToken,
                    ApiUrl = "https://api.soundcloud.com/",
                    IncludeClientIdInAuthRequests = false,
                    AuthenticationScheme = "OAuth"
                },
                new ServiceInfo
                {
                    Service = ServiceTypes.SoundCloudV2,
                    ClientIds = AppKeys.SoundCloudClientIds,
                    ClientId = AppKeys.SoundCloudClientId,
                    UserToken = soundCloudToken,
                    ApiUrl = "https://api-v2.soundcloud.com/",
                    IncludeClientIdInAuthRequests = false,
                    AuthenticationScheme = "OAuth"
                },
                new ServiceInfo
                {
                    Service = ServiceTypes.YouTube,
                    ClientId = AppKeys.YouTubeClientId,
                    UserToken = youTubeToken,
                    ApiUrl = "https://www.googleapis.com/youtube/v3/",
                    ClientIdName = "key",
                    AuthenticationScheme = "Bearer"
                },
                new ServiceInfo
                {
                    Service = ServiceTypes.ITunesPodcast,
                    ClientId = "0",
                    ApiUrl = "https://itunes.apple.com/",
                    ClientIdName = "key",
                },
                new ServiceInfo
                {
                    Service = ServiceTypes.Local
                }
            };

            SoundByteService.Current.Init(secretList);
        }

        #endregion App Setup

        #region Key Events

        private void CoreWindowOnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Set the ctrl pressed flag when it's pressed
            if (args.VirtualKey == VirtualKey.Control)
            {
                _isCtrlKeyPressed = true;
                return;
            }

            switch (args.VirtualKey)
            {
                case VirtualKey.F11:
                    // Send hit
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Toggle FullScreen");
                    // Toggle between full screen or not
                    if (!DeviceHelper.IsDeviceFullScreen)
                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                    else
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                    break;

                case VirtualKey.GamepadY:
                    // Send hit
                    SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Xbox Playing Page");

                    // Navigate to the current playing track
                    NavigateTo(typeof(NowPlayingView));
                    break;

                case VirtualKey.Back:
                    if (_isCtrlKeyPressed)
                    {
                        if (CurrentFrame?.Frame?.CanGoBack == true)
                        {
                            CurrentFrame?.Frame?.GoBack();
                        }
                    }
                    break;

                case VirtualKey.F:
                    // If the ctrl key is pressed, attempt to focus on the search box.
                    if (_isCtrlKeyPressed)
                    {
                        if ((Shell as Page)?.FindName("SearchBox") is AutoSuggestBox searchBox)
                            searchBox.Focus(FocusState.Keyboard);
                    }
                    break;
            }

            // Reset ctrl pressed
            _isCtrlKeyPressed = false;
        }

        #endregion Key Events

        /// <summary>
        ///     Creates the window and performs any protocol logic needed
        /// </summary>
        /// <param name="parameters">Parameter string, (soundbyte://core/user?id=454345)</param>
        /// <returns></returns>
        private async Task InitializeShellAsync(string? parameters = null)
        {
            _isInit = true;

            // If the shell is already setup, handle the protocol, otherwise create the shell.
            if (Window.Current.Content is DesktopShell || Window.Current.Content is XboxShell)
            {
                await SetLoadingAsync(true);
                await ProtocolHelper.HandleProtocolAsync(parameters);
                await SetLoadingAsync(false);
            }
            else
            {
                // Initialize the service
                InitV3Service();

                // This must be run asap
                ViewModelLocator.EarlyInit();

#if DEBUG
                // Opt Out
                await SimpleIoc.Default.GetInstance<ITelemetryService>().OptOutAsync();
#endif

                // If we are running on Xbox, or interactive mode is enable, load the
                // Xbox shell
                if (DeviceHelper.IsXbox || SettingsService.Instance.IsInteractiveMode)
                {
                    // Create the shell
                    Window.Current.Content = new XboxShell(parameters);

                    // Display the screen to the full width and height
                    ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
                }
                else
                {
                    // Live tile helpers
                    TileHelper.Init();

                    // Create the shell
                    Window.Current.Content = new DesktopShell(parameters);
                }

                // Hook the key pressed event for the global app
                Window.Current.CoreWindow.KeyDown += CoreWindowOnKeyDown;
            }

            // Activate the window
            Window.Current.Activate();
        }

        #region Static App Helpers

        /// <summary>
        ///     Navigate to a certain page using the main shells
        ///     root from navigate method
        /// </summary>
        public static void NavigateTo(Type page, object? param = null) =>
            NavigateToAsync(page, param).FireAndForgetSafeAsync();

        /// <summary>
        ///     Navigate to a certain page using the main shells
        ///     root from navigate method
        /// </summary>
        public static async Task NavigateToAsync(Type page, object param)
        {
            try
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    (Window.Current.Content as IAppShell)?.RootFrame?.Navigate(page, param);
                });
            }
            catch (Exception ex)
            {
                await NavigationService.Current.CallMessageDialogAsync("An error occurred while trying to navigate pages. Please try again. If this error continues to happen, please send the follow message to the developers:\n\n" + ex.Message, "Navigation Error");
            }
        }

        public static Page? CurrentFrame => (Window.Current?.Content as IAppShell)?.RootFrame.Content as Page;

        public static IAppShell? Shell => Window.Current?.Content as IAppShell;

        /// <summary>
        ///     Notification Manager for the app
        /// </summary>
        public static InAppNotification? NotificationManager => Shell?.GetName("NotificationManager") as InAppNotification;

        /// <summary>
        /// Updates the UI to either show a loading ring or not
        /// </summary>
        /// <param name="isLoading">Is the app loading</param>
        public static async Task SetLoadingAsync(bool isLoading)
        {
            // Don't run in background
            if (DeviceHelper.IsBackground)
                return;

            try
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    // Loading is only supported on the desktop shell.
                    if (!(Window.Current.Content is DesktopShell))
                        return;

                    if ((Window.Current?.Content as DesktopShell)?.FindName("LoadingRing") is ProgressBar loadingRing)
                        loadingRing.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
                });
            }
            catch
            {
                // This can crash if the UI thread does not exist.
                // 99.9% of the time, the background switch will prevent
                // this from happening though.
            }
        }

        #endregion Static App Helpers

        #region Background Handlers

        /// <summary>
        ///     The application is leaving the background.
        /// </summary>
        private void AppLeavingBackground(object sender, LeavingBackgroundEventArgs e) =>
            AppLeavingBackgroundAsync().FireAndForgetSafeAsync();

        private async Task AppLeavingBackgroundAsync()
        {
            // Mark the transition out of the background state
            DeviceHelper.IsBackground = false;

            // Send hit
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Leave Background");

            // Restore view content if it was previously unloaded
            if (Window.Current != null && Window.Current.Content == null && !_isInit)
            {
                await InitializeShellAsync();
            }
        }

        /// <summary>
        ///     The application entered the background.
        /// </summary>
        private void AppEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();

            try
            {
                // Send hit
                SimpleIoc.Default.GetInstance<ITelemetryService>().TrackEvent("Enter Background");

                // Update the variable
                DeviceHelper.IsBackground = true;
            }
            finally
            {
                deferral.Complete();
            }
        }

        #endregion Background Handlers

        #region Memory Handlers

        /// <summary>
        ///     Raised when the memory limit for the app is changing, such as when the app
        ///     enters the background.
        /// </summary>
        /// <remarks>
        ///     If the app is using more than the new limit, it must reduce memory within 2 seconds
        ///     on some platforms in order to avoid being suspended or terminated.
        ///     While some platforms will allow the application
        ///     to continue running over the limit, reducing usage in the time
        ///     allotted will enable the best experience across the broadest range of devices.
        /// </remarks>
        private void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
            => MemoryManager_AppMemoryUsageLimitChangingAsync(sender, e).FireAndForgetSafeAsync();

        private async Task MemoryManager_AppMemoryUsageLimitChangingAsync(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {
            // If app memory usage is over the limit, reduce usage within 2 seconds
            // so that the system does not suspend the app
            if (MemoryManager.AppMemoryUsage >= e.NewLimit)
                await ReduceMemoryUsageAsync();
        }

        /// <summary>
        ///     Handle system notifications that the app has increased its
        ///     memory usage level compared to its current target.
        /// </summary>
        /// <remarks>
        ///     The app may have increased its usage or the app may have moved
        ///     to the background and the system lowered the target for the app
        ///     In either case, if the application wants to maintain its priority
        ///     to avoid being suspended before other apps, it may need to reduce
        ///     its memory usage.
        ///     This is not a replacement for handling AppMemoryUsageLimitChanging
        ///     which is critical to ensure the app immediately gets below the new
        ///     limit. However, once the app is allowed to continue running and
        ///     policy is applied, some apps may wish to continue monitoring
        ///     usage to ensure they remain below the limit.
        /// </remarks>
        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e) =>
            MemoryManager_AppMemoryUsageIncreasedAsync(sender, e).FireAndForgetSafeAsync();

        private async Task MemoryManager_AppMemoryUsageIncreasedAsync(object sender, object e)
        {
            // Obtain the current usage level
            var level = MemoryManager.AppMemoryUsageLevel;

            // Check the usage level to determine whether reducing memory is necessary.
            // Memory usage may have been fine when initially entering the background but
            // the app may have increased its memory usage since then and will need to trim back.
            if (level == AppMemoryUsageLevel.OverLimit || level == AppMemoryUsageLevel.High)
                await ReduceMemoryUsageAsync();
        }

        /// <summary>
        ///     Reduces application memory usage.
        /// </summary>
        /// <remarks>
        ///     When the app enters the background, receives a memory limit changing
        ///     event, or receives a memory usage increased event, it can
        ///     can optionally unload cached data or even its view content in
        ///     order to reduce memory usage and the chance of being suspended.
        ///     This must be called from multiple event handlers because an application may already
        ///     be in a high memory usage state when entering the background, or it
        ///     may be in a low memory usage state with no need to unload resources yet
        ///     and only enter a higher state later.
        /// </remarks>
        private async Task ReduceMemoryUsageAsync()
        {
            try
            {
                // Run on UI thread
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Additionally, if the application is currently
                    // in background mode and still has a view with content
                    // then the view can be released to save memory and
                    // can be recreated again later when leaving the background.
                    if (Window.Current == null || Window.Current.Content == null)
                        return;

                    // Delete all view models
                    (App.Current.Resources["ViewModelLocator"] as ViewModelLocator)?.CleanViewModels();

                    // If desktop shell
                    if (Window.Current.Content is DesktopShell desktopShell)
                    {
                        // Dispose the shell
                        desktopShell.Dispose();

                        // Clear the page cache
                        var cacheSize = desktopShell.RootFrame.CacheSize;
                        desktopShell.RootFrame.CacheSize = 0;
                        desktopShell.RootFrame.CacheSize = cacheSize;

                        // Clear back-stack
                        desktopShell.RootFrame.BackStack.Clear();

                        // Clear references
                        VisualTreeHelper.DisconnectChildrenRecursive(desktopShell.RootFrame);
                        VisualTreeHelper.DisconnectChildrenRecursive(desktopShell);
                    }

                    // If xbox shell
                    if (Window.Current.Content is XboxShell xboxShell)
                    {
                        // Dispose the shell
                        xboxShell.Dispose();

                        // Clear the page cache
                        var cacheSize = xboxShell.RootFrame.CacheSize;
                        xboxShell.RootFrame.CacheSize = 0;
                        xboxShell.RootFrame.CacheSize = cacheSize;

                        // Clear back-stack
                        xboxShell.RootFrame.BackStack.Clear();

                        // Clear references
                        VisualTreeHelper.DisconnectChildrenRecursive(xboxShell.RootFrame);
                        VisualTreeHelper.DisconnectChildrenRecursive(xboxShell);
                    }

                    // Clear the view content. Note that views should rely on
                    // events like Page.Unloaded to further release resources.
                    // Release event handlers in views since references can
                    // prevent objects from being collected.
                    Window.Current.Content = null;

                    // Allow us to init the window again
                    _isInit = false;

                    GC.Collect();
                });
            }
            catch
            {
                // This will crash if no main view is active
            }

            // Run the GC to collect released resources on background thread.
            GC.Collect();
        }

        #endregion Memory Handlers

        #region Launch / Activate Events

        /// <summary>
        ///     Called when the app is activated.
        /// </summary>
        protected override void OnActivated(IActivatedEventArgs e) =>
            OnActivatedAsync(e).FireAndForgetSafeAsync();

        private async Task OnActivatedAsync(IActivatedEventArgs e)
        {
            var path = string.Empty;

            // Handle all the activation protocols that could occur
            switch (e.Kind)
            {
                // We were launched using the protocol
                case ActivationKind.Protocol:
                    if (e is ProtocolActivatedEventArgs protoArgs)
                        path = protoArgs.Uri.ToString();
                    break;

                case ActivationKind.ToastNotification:
                    if (e is IToastNotificationActivatedEventArgs toastArgs)
                        path = toastArgs.Argument;
                    break;

                case ActivationKind.DialReceiver:
                    if (e is DialReceiverActivatedEventArgs dialReceiverArgs)
                        path = dialReceiverArgs.Arguments;
                    break;

                case ActivationKind.VoiceCommand:
                    if (e is VoiceCommandActivatedEventArgs voiceArgs)
                        path = "sb://cortana?command=" + voiceArgs.Result.RulePath[0];
                    break;
            }

            await InitializeShellAsync(path);
        }

        /// <summary>
        ///     Invoked when the application is launched normally by the end user.  Other entry points
        ///     will be used such as when the application is launched to open a specific file.
        /// </summary>
        protected override void OnLaunched(LaunchActivatedEventArgs e) =>
            OnLaunchedAsync(e).FireAndForgetSafeAsync();

        private async Task OnLaunchedAsync(LaunchActivatedEventArgs e)
        {
            var path = string.Empty;

            // Handle all the activation protocols that could occur
            if (!string.IsNullOrEmpty(e.TileId))
                path = e.Arguments;

            // If this is just a pre launch, don't
            // actually set the content to the frame.
            if (e.PrelaunchActivated) return;

            // Track app use
            SystemInformation.TrackAppUse(e);

            // Create / Get the main shell
            await InitializeShellAsync(path);
        }

        protected override void OnFileActivated(FileActivatedEventArgs args) =>
            OnFileActivatedAsync(args).FireAndForgetSafeAsync();

        private async Task OnFileActivatedAsync(FileActivatedEventArgs args)
        {
            // Start playback
            await LocalPlaybackHelper.StartLocalPlaybackAsync(args.Files);

            // Load the shell
            await InitializeShellAsync();
        }

        #endregion Launch / Activate Events
    }
}

#nullable restore