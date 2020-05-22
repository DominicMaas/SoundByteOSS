using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.App.Uwp.Dialogs;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Views;
using SoundByte.App.Uwp.Views.Playback;
using SoundByte.App.Uwp.Views.Xbox;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SoundByte.App.Uwp.ViewModels.Playback
{
    public class PlaybackViewModel : BaseViewModel
    {
        private readonly CoreDispatcher _currentUiDispatcher;

        #region Getters and Setters

        /// <summary>
        ///     Current playlist of items
        /// </summary>
        public ObservableCollection<BaseSoundByteItem> Playlist { get; } = new ObservableCollection<BaseSoundByteItem>();

        /// <summary>
        ///     Is reposting enabled (only soundcloud)
        /// </summary>
        public bool IsRepostEnabled
        {
            get => _isRepostEnabled;
            set
            {
                if (_isRepostEnabled == value)
                    return;

                _isRepostEnabled = value;
                UpdateProperty();
            }
        }

        private bool _isRepostEnabled;

        /// <summary>
        ///     Is the remote system enabled
        /// </summary>
        public bool IsRemoteFlyoutOpen
        {
            get => _isRemoteFlyoutOpen;
            set
            {
                if (_isRemoteFlyoutOpen == value)
                    return;

                _isRemoteFlyoutOpen = value;
                UpdateProperty();
            }
        }

        private bool _isRemoteFlyoutOpen;

        /// <summary>
        ///     Has the tile been pined to the start menu
        /// </summary>
        public bool IsTilePined
        {
            get => _isTilePinned;
            set
            {
                if (_isTilePinned == value)
                    return;

                _isTilePinned = value;
                UpdateProperty();
            }
        }

        private bool _isTilePinned;

        /// <summary>
        /// The current playing track
        /// </summary>
        public BaseTrack CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack == value)
                    return;

                _currentTrack = value;
                UpdateProperty();
            }
        }

        private BaseTrack _currentTrack;

        /// <summary>
        ///     The amount of time spent listening to the track
        /// </summary>
        public string TimeListened
        {
            get => _timeListened;
            set
            {
                if (_timeListened == value)
                    return;

                _timeListened = value;
                UpdateProperty();
            }
        }

        private string _timeListened = "00:00";

        /// <summary>
        ///     The amount of time remaining
        /// </summary>
        public string TimeRemaining
        {
            get => _timeRemaining;
            set
            {
                if (_timeRemaining == value)
                    return;

                _timeRemaining = value;
                UpdateProperty();
            }
        }

        private string _timeRemaining = "-00:00";

        /// <summary>
        ///     The current slider value
        /// </summary>
        public double CurrentTimeValue
        {
            get => _currentTimeValue;
            set
            {
                _currentTimeValue = value;
                UpdateProperty();
            }
        }

        private double _currentTimeValue;

        /// <summary>
        ///     The max slider value
        /// </summary>
        public double MaxTimeValue
        {
            get => _maxTimeValue;
            private set
            {
                _maxTimeValue = value;
                UpdateProperty();
            }
        }

        private double _maxTimeValue = 100;

        /// <summary>
        ///     The current text for the volume icon
        /// </summary>
        public string VolumeIcon
        {
            get => _volumeIcon;
            private set
            {
                if (_volumeIcon == value)
                    return;

                _volumeIcon = value;
                UpdateProperty();
            }
        }

        private string _volumeIcon = "\uE767";

        /// <summary>
        ///     The content on the play_pause button
        /// </summary>
        public string PlayButtonContent
        {
            get => _playButtonContent;
            set
            {
                if (_playButtonContent == value)
                    return;

                _playButtonContent = value;
                UpdateProperty();
            }
        }

        private string _playButtonContent = "\uE769";

        /// <summary>
        ///     The current value of the volume slider
        /// </summary>
        public double MediaVolume
        {
            get => SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackVolume() * 100;
            set
            {
                // Set the volume
                SimpleIoc.Default.GetInstance<IPlaybackService>().SetTrackVolume(value / 100);

                // Save the volume
                SettingsService.Instance.PlaybackVolume = value;

                // Update the UI
                if ((int)value == 0)
                {
                    SimpleIoc.Default.GetInstance<IPlaybackService>().MuteTrack(true);
                    VolumeIcon = "\uE74F";
                }
                else if (value < 25)
                {
                    SimpleIoc.Default.GetInstance<IPlaybackService>().MuteTrack(false);
                    VolumeIcon = "\uE992";
                }
                else if (value < 50)
                {
                    SimpleIoc.Default.GetInstance<IPlaybackService>().MuteTrack(false);
                    VolumeIcon = "\uE993";
                }
                else if (value < 75)
                {
                    SimpleIoc.Default.GetInstance<IPlaybackService>().MuteTrack(false);
                    VolumeIcon = "\uE994";
                }
                else
                {
                    SimpleIoc.Default.GetInstance<IPlaybackService>().MuteTrack(false);
                    VolumeIcon = "\uE767";
                }

                UpdateProperty();
            }
        }

        /// <summary>
        ///     Are tracks shuffled
        /// </summary>
        public bool IsShuffleEnabled
        {
            get => _isShuffledEnabled;
            set
            {
                _isShuffledEnabled = value;
                UpdateProperty();
            }
        }

        private bool _isShuffledEnabled;

        /// <summary>
        ///     Is the song going to repeat when finished
        /// </summary>
        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set
            {
                _isRepeatEnabled = value;
                UpdateProperty();
            }
        }

        private bool _isRepeatEnabled;

        #endregion Getters and Setters

        #region Timers

        /// <summary>
        ///     This timer runs every 500ms to ensure that the current position,
        ///     time, remaining time, etc. variables are correct.
        /// </summary>
        private readonly DispatcherTimer _updateInformationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        /// <summary>
        ///     This timer has to run quite fast. It ensures that audio and video are
        ///     in sync for YouTube videos.
        /// </summary>
        private readonly DispatcherTimer _audioVideoSyncTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(75)
        };

        #endregion Timers

        #region Constructors

        [PreferredConstructor]
        public PlaybackViewModel() : this(CoreApplication.MainView.Dispatcher)
        { }

        public PlaybackViewModel(CoreDispatcher uiDispatcher)
        {
            _currentUiDispatcher = uiDispatcher;

            // Bind the methods that we need
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnStateChange += OnStateChange;
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange += OnTrackChange;

            // Bind timer methods
            _updateInformationTimer.Tick += UpdateInformation;
            _audioVideoSyncTimer.Tick += SyncAudioVideo;

            // Update info to current track
            OnTrackChange(SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack());
            UpdateUpNext();
            MediaVolume = MediaVolume;

            Application.Current.LeavingBackground += CurrentOnLeavingBackground;

            // Start the timer if ready
            if (!_updateInformationTimer.IsEnabled)
                _updateInformationTimer.Start();
        }

        private void CurrentOnLeavingBackground(object sender, LeavingBackgroundEventArgs leavingBackgroundEventArgs)
        {
            // Call the track change method
            OnTrackChange(SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack());
        }

        #endregion Constructors

        #region Timer Methods

        /// <summary>
        ///     Syncs YouTube audio and video when needed
        /// </summary>
        private async void SyncAudioVideo(object sender, object e)
        {
            // Only run this method if there is a track and it's a
            // youtube track
            if (CurrentTrack == null || CurrentTrack.ServiceType != ServiceTypes.YouTube)
                return;

            // Don't run in the background
            if (DeviceHelper.IsBackground)
                return;

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaybackState() != MediaPlaybackState.Playing
                || SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition().Milliseconds <= 0)
                return;

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!(App.CurrentFrame?.FindName("VideoOverlay") is MediaElement overlay))
                    return;

                var difference = overlay.Position - SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition();

                if (Math.Abs(difference.TotalMilliseconds) >= 1000)
                {
                    overlay.PlaybackRate = 1;
                    overlay.Position = SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition();
                    System.Diagnostics.Debug.WriteLine("OUT OF SYNC: SKIPPING (>= 1000ms)");
                }
                else if (Math.Abs(difference.TotalMilliseconds) >= 500)
                {
                    overlay.PlaybackRate = difference.TotalMilliseconds > 0 ? 0.25 : 1.75;
                    System.Diagnostics.Debug.WriteLine("OUT OF SYNC: CHANGE PLAYBACK RATE (>= 500ms)");
                }
                else if (Math.Abs(difference.TotalMilliseconds) >= 250)
                {
                    overlay.PlaybackRate = difference.TotalMilliseconds > 0 ? 0.5 : 1.5;
                    System.Diagnostics.Debug.WriteLine("OUT OF SYNC: CHANGE PLAYBACK RATE (>= 250ms)");
                }
                else if (Math.Abs(difference.TotalMilliseconds) >= 100)
                {
                    overlay.PlaybackRate = difference.TotalMilliseconds > 0 ? 0.75 : 1.25;
                    System.Diagnostics.Debug.WriteLine("OUT OF SYNC: CHANGE PLAYBACK RATE (>= 100ms)");
                }
                else
                {
                    overlay.PlaybackRate = 1;
                }
            });
        }

        /// <summary>
        ///     Timer method that is run to make sure the UI is kept up to date
        /// </summary>
        private async void UpdateInformation(object sender, object e)
        {
            if (DeviceHelper.IsBackground)
                return;

            // Only call the following if the player exists, is playing
            // and the time is greater then 0.
            if (SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaybackState() != MediaPlaybackState.Playing
                || SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition().Milliseconds <= 0)
                return;

            if (CurrentTrack == null)
                return;

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (CurrentTrack.IsLive)
                {
                    // Set the current time value
                    CurrentTimeValue = 1;

                    // Set the time listened text
                    TimeListened = "LIVE";

                    // Set the time remaining text
                    TimeRemaining = "LIVE";

                    // Set the maximum value
                    MaxTimeValue = 1;
                }
                else
                {
                    // Set the current time value
                    CurrentTimeValue = SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition().TotalSeconds;

                    // Get the remaining time for the track
                    var remainingTime = SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackDuration().Subtract(SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition());

                    // Set the time listened text
                    TimeListened = NumberFormatHelper.FormatTimeString(SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackPosition().TotalMilliseconds);

                    // Set the time remaining text
                    TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(remainingTime.TotalMilliseconds);

                    // Set the maximum value
                    MaxTimeValue = SimpleIoc.Default.GetInstance<IPlaybackService>().GetTrackDuration().TotalSeconds;
                }
            });
        }

        #endregion Timer Methods

        #region Track Control Methods

        /// <summary>
        ///     Toggle if the current track should repeat
        /// </summary>
        public void ToggleRepeat()
        {
            if (SimpleIoc.Default.GetInstance<IPlaybackService>().IsTrackRepeating())
            {
                IsRepeatEnabled = false;
                SimpleIoc.Default.GetInstance<IPlaybackService>().RepeatTrack(false);
            }
            else
            {
                IsRepeatEnabled = true;
                SimpleIoc.Default.GetInstance<IPlaybackService>().RepeatTrack(true);
            }
        }

        /// <summary>
        ///     Toggle if the current playlist is shuffled
        /// </summary>
        public async void ToggleShuffle()
        {
            if (SimpleIoc.Default.GetInstance<IPlaybackService>().IsPlaylistShuffled())
            {
                IsShuffleEnabled = false;
                await SimpleIoc.Default.GetInstance<IPlaybackService>().ShufflePlaylistAsync(false);
            }
            else
            {
                IsShuffleEnabled = true;
                await SimpleIoc.Default.GetInstance<IPlaybackService>().ShufflePlaylistAsync(true);
            }

            UpdateUpNext();
        }

        /// <summary>
        ///     Toggle if we should mute
        /// </summary>
        public void ToggleMute()
        {
            // Toggle mute
            SimpleIoc.Default.GetInstance<IPlaybackService>().MuteTrack(!SimpleIoc.Default.GetInstance<IPlaybackService>().IsTrackMuted());

            // Update the UI
            VolumeIcon = SimpleIoc.Default.GetInstance<IPlaybackService>().IsTrackMuted() ? "\uE74F" : "\uE767";
        }

        public void NavigateNowPlaying()
        {
            App.NavigateTo(typeof(XboxPlayingView));
        }

        public void NavigateNowPlayingInfo()
        {
            App.NavigateTo(typeof(XboxPlayingView), "track-info");
        }

        #endregion Track Control Methods

        #region Track Playback State

        /// <summary>
        ///     Toggles the state between the track playing
        ///     and not playing
        /// </summary>
        public void ChangePlaybackState()
        {
            // Get the current state of the track
            var currentState = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaybackState();

            // If the track is currently paused
            if (currentState == MediaPlaybackState.Paused)
            {
                // Play the track
                SimpleIoc.Default.GetInstance<IPlaybackService>().PlayTrack();
            }

            // If the track is currently playing
            if (currentState == MediaPlaybackState.Playing)
            {
                // Pause the track
                SimpleIoc.Default.GetInstance<IPlaybackService>().PauseTrack();
            }
        }

        /// <summary>
        ///     Go forward one track
        /// </summary>
        public void SkipNext()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().NextTrack();
        }

        /// <summary>
        ///     Go backwards one track
        /// </summary>
        public void SkipPrevious()
        {
            SimpleIoc.Default.GetInstance<IPlaybackService>().PreviousTrack();
        }

        #endregion Track Playback State

        private void UpdateUpNext()
        {
            // Get and convert tracks
            var playlist = SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylist();

            // Clear playlist and add items
            Playlist.Clear();
            foreach (var baseTrack in playlist)
            {
                Playlist.Add(new BaseSoundByteItem(baseTrack));
            }
        }

        #region Methods

        public async void OnPlayingSliderChange()
        {
            if (DeviceHelper.IsBackground)
                return;

            if (CurrentTrack == null)
                return;

            if (CurrentTrack.IsLive)
                return;

            // Set the track position
            SimpleIoc.Default.GetInstance<IPlaybackService>().SetTrackPosition(TimeSpan.FromSeconds(CurrentTimeValue));

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!(App.CurrentFrame?.FindName("VideoOverlay") is MediaElement overlay))
                    return;

                overlay.Position = TimeSpan.FromSeconds(CurrentTimeValue);
            });
        }

        /// <summary>
        ///     Called when the playback service loads a new track. Used
        ///     to update the required values for the UI.
        /// </summary>
        /// <param name="track"></param>
        private async void OnTrackChange(BaseTrack track)
        {
            // Do nothing if running in the background
            if (DeviceHelper.IsBackground)
                return;

            // Same track, no need to perform this logic
            if (track == CurrentTrack)
                return;

            // Run all this on the UI thread
            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                // Set the new current track, updating the UI
                CurrentTrack = track;

                // Only run the sync timer when listening to a youtube music video
                if (track.ServiceType == ServiceTypes.YouTube)
                {
                    if (!_audioVideoSyncTimer.IsEnabled)
                        _audioVideoSyncTimer.Start();
                }
                else
                {
                    if (_audioVideoSyncTimer.IsEnabled)
                        _audioVideoSyncTimer.Stop();
                }

                if (!track.IsLive)
                {
                    TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(track.Duration.TotalMilliseconds);
                    TimeListened = "00:00";
                    CurrentTimeValue = 0;
                    MaxTimeValue = track.Duration.TotalSeconds;
                }
                else
                {
                    TimeRemaining = "LIVE";
                    TimeListened = "LIVE";
                    CurrentTimeValue = 1;
                    MaxTimeValue = 1;
                }

                UpdateUpNext();

                // If the current track is a soundcloud track, enabled reposting.
                IsRepostEnabled = track.ServiceType == ServiceTypes.SoundCloud;

                // Update the tile value
                IsTilePined = TileHelper.IsTilePinned("Track_" + track.TrackId);

                if (CurrentTrack?.ServiceType == ServiceTypes.SoundCloud)
                {
                    try
                    {
                        CurrentTrack.User = (await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceTypes.SoundCloud, $"/users/{CurrentTrack.User.UserId}")).Response.ToBaseUser();
                    }
                    catch
                    {
                        // ignored
                    }
                }
            });
        }

        private async void OnStateChange(MediaPlaybackState mediaPlaybackState)
        {
            // Don't run in the background if on Xbox
            if (DeviceHelper.IsBackground && DeviceHelper.IsXbox)
                return;

            await _currentUiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var overlay = App.CurrentFrame?.FindName("VideoOverlay") as MediaElement;

                switch (mediaPlaybackState)
                {
                    case MediaPlaybackState.Playing:
                        if (!DeviceHelper.IsDesktop)
                        {
                            await App.SetLoadingAsync(false);
                        }
                        PlayButtonContent = "\uE769";
                        overlay?.Play();
                        break;

                    case MediaPlaybackState.Buffering:
                    case MediaPlaybackState.Opening:
                        if (!DeviceHelper.IsDesktop)
                        {
                            await App.SetLoadingAsync(true);
                        }
                        break;

                    case MediaPlaybackState.None:
                        if (!DeviceHelper.IsDesktop)
                        {
                            await App.SetLoadingAsync(false);
                        }
                        PlayButtonContent = "\uE768";
                        break;

                    case MediaPlaybackState.Paused:
                        if (!DeviceHelper.IsDesktop)
                        {
                            await App.SetLoadingAsync(false);
                        }
                        PlayButtonContent = "\uE768";
                        overlay?.Pause();
                        break;

                    default:
                        if (!DeviceHelper.IsDesktop)
                        {
                            await App.SetLoadingAsync(false);
                        }
                        PlayButtonContent = "\uE768";
                        overlay?.Play();
                        break;
                }
            });
        }

        #endregion Methods

        public override void Dispose()
        {
            // Unbind the methods that we need
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnStateChange -= OnStateChange;
            SimpleIoc.Default.GetInstance<IPlaybackService>().OnTrackChange -= OnTrackChange;

            // Unbind timer methods
            _updateInformationTimer.Tick -= UpdateInformation;
            _audioVideoSyncTimer.Tick -= SyncAudioVideo;

            Application.Current.LeavingBackground -= CurrentOnLeavingBackground;

            //if (_remoteSystemSessionWatcher != null)
            //{
            //    // Unbind methods
            //    _remoteSystemSessionWatcher.Added += RemoteSystemWatcher_RemoteSystemAdded;
            //    _remoteSystemSessionWatcher.Removed += RemoteSystemWatcher_RemoteSystemRemoved;
            //    _remoteSystemSessionWatcher.Updated += RemoteSystemWatcher_RemoteSystemUpdated;

            //    _remoteSystemSessionWatcher.Stop();
            //}

            base.Dispose();
        }

        #region Method Bindings

        /// <summary>
        ///     Toggle if the track has been liked or not
        /// </summary>
        public async void ToggleLikeTrack()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Like Error");
                return;
            }

            // User is not logged in
            if (!SoundByteService.Current.IsServiceConnected(CurrentTrack.ServiceType))
            {
                await NavigationService.Current.CallMessageDialogAsync($"You must connect your {CurrentTrack.ServiceType} account to do this.", "Like Error");
                return;
            }

            // Toggle like status
            CurrentTrack.ToggleLike();
        }

        /// <summary>
        ///     Toggle is a track has been reposted or not
        /// </summary>
        public async void ToggleRepostTrack()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Repost Error");
                return;
            }

            // Must be SoundCloud track
            if (CurrentTrack.ServiceType != ServiceTypes.SoundCloud &&
                CurrentTrack.ServiceType != ServiceTypes.SoundCloudV2)
            {
                await NavigationService.Current.CallMessageDialogAsync("Reposting is only supported on SoundCloud tracks.", "Repost Error");
                return;
            }

            // User is not logged in
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
            {
                await NavigationService.Current.CallMessageDialogAsync("You must connect your SoundCloud account to do this.", "Repost Error");
                return;
            }

            try
            {
                if (!(await SoundByteService.Current.PutAsync(ServiceTypes.SoundCloud, $"/e1/me/track_reposts/{CurrentTrack.TrackId}")).Response)
                {
                    await NavigationService.Current.CallMessageDialogAsync("Unknown Error", "Repost Error");
                }
            }
            catch (Exception e)
            {
                await NavigationService.Current.CallMessageDialogAsync(e.Message, "Repost Error");
            }
        }

        public async void TogglePinTile()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Pin Tile Error");
                return;
            }

            // Check if the tile exists
            var tileExists = TileHelper.IsTilePinned("Track_" + CurrentTrack.TrackId);

            if (tileExists)
            {
                // Remove the tile and check if it was successful
                if (await TileHelper.RemoveTileAsync("Track_" + CurrentTrack.TrackId))
                {
                    IsTilePined = false;
                }
                else
                {
                    IsTilePined = true;
                }
            }
            else
            {
                // Create a live tile and check if it was created
                if (await TileHelper.CreateTileAsync("Track_" + CurrentTrack.TrackId,
                    CurrentTrack.Title, $"soundbyte://track?d={ProtocolHelper.EncodeTrackProtocolItem(new ProtocolHelper.TrackProtocolItem(SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylistSource(), new BaseSoundByteItem(CurrentTrack), null, SimpleIoc.Default.GetInstance<IPlaybackService>().GetPlaylistToken()), true)}",
                new Uri(CurrentTrack.ThumbnailUrl), ForegroundText.Light))
                {
                    IsTilePined = true;
                }
                else
                {
                    IsTilePined = false;
                }
            }
        }

        /// <summary>
        ///     Display the playlist dialog so the user can
        ///     add the current song to a playlist.
        /// </summary>
        public async void DisplayPlaylist()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Error");
                return;
            }

            await NavigationService.Current.CallDialogAsync<AddToPlaylistDialog>(SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack());
        }

        /// <summary>
        ///     Open the share track dialog
        /// </summary>
        public async void ShareTrack()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Error");
                return;
            }

            await NavigationService.Current.CallDialogAsync<ShareDialog>(SimpleIoc.Default.GetInstance<IPlaybackService>().GetCurrentTrack());
        }

        /// <summary>
        ///     View the artist that created this track.
        /// </summary>
        public async void ViewArtist()
        {
            // Track must exist
            if (CurrentTrack == null)
            {
                await NavigationService.Current.CallMessageDialogAsync("No track is currently playing.", "Error");
                return;
            }

            App.NavigateTo(typeof(UserView), CurrentTrack.User);
        }

        /// <summary>
        ///     Switch to compact overlay mode
        /// </summary>
        public async void SwitchToCompactView()
        {
            try
            {
                var compactView = CoreApplication.CreateNewView();
                var compactViewId = -1;
                var currentViewId = -1;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Get the Id back
                    currentViewId = ApplicationView.GetForCurrentView().Id;
                });

                // Create a new window within the view
                await compactView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    // Create a new frame and navigate it to the overlay view
                    var overlayFrame = new Frame();
                    overlayFrame.Navigate(typeof(OverlayView), currentViewId);

                    // Set the window content and activate it
                    Window.Current.Content = overlayFrame;
                    Window.Current.Activate();

                    // Get the Id back
                    compactViewId = ApplicationView.GetForCurrentView().Id;
                });

                // Make the overlay small
                var compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
                compactOptions.CustomSize = new Size(300, 430);

                // Display as compact overlay
                await ApplicationViewSwitcher.TryShowAsViewModeAsync(compactViewId, ApplicationViewMode.CompactOverlay,
                    compactOptions);

                // Switch to this window
                await ApplicationViewSwitcher.SwitchAsync(compactViewId, currentViewId,
                    ApplicationViewSwitchingOptions.ConsolidateViews);
            }
            catch (Exception e)
            {
                await NavigationService.Current.CallMessageDialogAsync("An error occurred while trying to switch to compact mode. More information:\n" + e.Message, "Compact Mode Error");
            }
        }

        #endregion Method Bindings
    }
}