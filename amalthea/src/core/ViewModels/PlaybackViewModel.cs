using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Helpers;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Playback;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.ViewModels.Details;
using Xamarin.Essentials;

namespace SoundByte.Core.ViewModels
{
    /// <summary>
    ///     View model for the now playing / playback screen, displays
    ///     the currently playing song alongside music controls
    /// </summary>
    public class PlaybackViewModel : MvxViewModel
    {
        private readonly IPlaybackService _playbackService;
        private readonly IMvxMainThreadAsyncDispatcher _mainThreadDispatcher;
        private readonly ITrackService _trackService;

        private readonly Timer _updateInformationTimer;

        #region Commands

        /// <summary>
        ///     This command is called from the now playing bar, and not from the page its
        ///     self
        /// </summary>
        public IMvxAsyncCommand NavigateNowPlayingPageCommand { get; }

        public IMvxAsyncCommand ShareCommand { get; }
        public IMvxAsyncCommand ManagePlaylistCommand { get; }
        public IMvxAsyncCommand ViewUserCommand { get; }
        public IMvxCommand ToggleShuffleCommand { get; }
        public IMvxCommand ToggleRepeatCommand { get; }
        public IMvxCommand ToggleMuteCommand { get; }
        public IMvxAsyncCommand ToggleTrackLikeCommand { get; }
        public IMvxCommand SkipPreviousCommand { get; }
        public IMvxCommand SkipNextCommand { get; }
        public IMvxCommand<double> ChangePlaybackRateCommand { get; }
        public IMvxCommand ToggleMediaStateCommand { get; }
        public IMvxCommand MoveMediaPosition { get; }
        public IMvxAsyncCommand CloseCommand { get; }

        #endregion Commands

        #region Getters & Setters

        private static string PlayIcon => DeviceInfo.Platform == DevicePlatform.UWP ? "\uE768" : "\uf04b";
        private static string PauseIcon => DeviceInfo.Platform == DevicePlatform.UWP ? "\uE769" : "\uf04c";

        public ObservableCollection<Track> PreviouslyPlayed { get; } = new ObservableCollection<Track>();
        public ObservableCollection<Track> UpNext { get; } = new ObservableCollection<Track>();

        /// <summary>
        ///     The current media playing at the moment
        /// </summary>
        public Track CurrentTrack
        {
            get => _currentTrack;
            set => SetProperty(ref _currentTrack, value);
        }

        private Track _currentTrack;

        /// <summary>
        ///     If the media is currently loading
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private bool _isLoading;

        /// <summary>
        ///     The amount of time spent listening to the media
        /// </summary>
        public string TimeListened
        {
            get => _timeListened;
            set => SetProperty(ref _timeListened, value);
        }

        private string _timeListened = "00:00";

        /// <summary>
        ///     The amount of time remaining for this media
        /// </summary>
        public string TimeRemaining
        {
            get => _timeRemaining;
            set => SetProperty(ref _timeRemaining, value);
        }

        private string _timeRemaining = "-00:00";

        /// <summary>
        ///     The current slider value
        /// </summary>
        public double CurrentTimeValue
        {
            get => _currentTimeValue;
            set => SetProperty(ref _currentTimeValue, value);
        }

        private double _currentTimeValue;

        /// <summary>
        ///     The max slider value
        /// </summary>
        public double MaxTimeValue
        {
            get => _maxTimeValue;
            set => SetProperty(ref _maxTimeValue, value);
        }

        private double _maxTimeValue = 100;

        /// <summary>
        ///     Are tracks shuffled
        /// </summary>
        public bool IsShuffleEnabled
        {
            get => _isShuffledEnabled;
            set => SetProperty(ref _isShuffledEnabled, value);
        }

        private bool _isShuffledEnabled;

        /// <summary>
        ///     Is the song going to repeat when finished
        /// </summary>
        public bool IsRepeatEnabled
        {
            get => _isRepeatEnabled;
            set => SetProperty(ref _isRepeatEnabled, value);
        }

        private bool _isRepeatEnabled;

        /// <summary>
        ///     If the track has been liked
        /// </summary>
        public bool IsTrackLiked
        {
            get => _isTrackLiked;
            set => SetProperty(ref _isTrackLiked, value);
        }

        private bool _isTrackLiked;

        /// <summary>
        ///     The current glyph for the volume icon
        /// </summary>
        public string VolumeIcon
        {
            get => _volumeIcon;
            private set => SetProperty(ref _volumeIcon, value);
        }

        private string _volumeIcon = "\uE767";

        /// <summary>
        ///     The icon of the play / pause button
        /// </summary>
        public string PlayButtonIcon
        {
            get => _playButtonIcon;
            set => SetProperty(ref _playButtonIcon, value);
        }

        private string _playButtonIcon = PauseIcon;

        /// <summary>
        ///     The current value of the volume slider
        /// </summary>
        public double MediaVolume
        {
            get => _playbackService.GetMediaVolume() * 100;
            set
            {
                // Set the volume
                _playbackService.SetMediaVolume(value / 100);

                // Save the volume
                // SettingsService.Instance.PlaybackVolume = value;

                // Update the UI
                if ((int)value == 0)
                {
                    _playbackService.MuteMedia(true);
                    VolumeIcon = "\uE74F";
                }
                else if (value < 25)
                {
                    _playbackService.MuteMedia(false);
                    VolumeIcon = "\uE992";
                }
                else if (value < 50)
                {
                    _playbackService.MuteMedia(false);
                    VolumeIcon = "\uE993";
                }
                else if (value < 75)
                {
                    _playbackService.MuteMedia(false);
                    VolumeIcon = "\uE994";
                }
                else
                {
                    _playbackService.MuteMedia(false);
                    VolumeIcon = "\uE767";
                }

                RaisePropertyChanged();
            }
        }

        #endregion Getters & Setters

        public PlaybackViewModel(IPlaybackService playbackService, IMvxMainThreadAsyncDispatcher mainThreadDispatcher, ITrackService trackService, IMvxNavigationService navigationService)
        {
            // Services
            _playbackService = playbackService;
            _mainThreadDispatcher = mainThreadDispatcher;
            _trackService = trackService;

            // Setup the timer
            _updateInformationTimer = new Timer(500);
            _updateInformationTimer.Elapsed += UpdateInformation;

            // Events
            _playbackService.OnStateChange += PlaybackService_OnStateChange;
            _playbackService.OnMediaChange += PlaybackService_OnMediaChange;

            // Commands
            NavigateNowPlayingPageCommand = new MvxAsyncCommand(async () => await navigationService.Navigate<PlaybackViewModel>());
            //ShareCommand = new MvxAsyncCommand(async () => await navigationService.Navigate<PlaybackViewModel>());
            //ManagePlaylistCommand = new MvxAsyncCommand(async () => await navigationService.Navigate<PlaybackViewModel>());
            ViewUserCommand = new MvxAsyncCommand(async () => await navigationService.Navigate<UserDetailViewModel, User>(CurrentTrack.User));
            ChangePlaybackRateCommand = new MvxCommand<double>(rate => { _playbackService.SetMediaPlaybackRate(rate); });
            ToggleShuffleCommand = new MvxAsyncCommand(async () =>
            {
                if (_playbackService.IsQueueShuffled())
                {
                    IsShuffleEnabled = false;
                    await _playbackService.ShuffleQueueAsync(false);
                }
                else
                {
                    IsShuffleEnabled = true;
                    await _playbackService.ShuffleQueueAsync(true);
                }

                UpdateUpNext();
            });
            ToggleRepeatCommand = new MvxCommand(() =>
            {
                if (_playbackService.IsMediaRepeating())
                {
                    IsRepeatEnabled = false;
                    _playbackService.RepeatMedia(false);
                }
                else
                {
                    IsRepeatEnabled = true;
                    _playbackService.RepeatMedia(true);
                }
            });
            ToggleMuteCommand = new MvxCommand(() =>
            {
                // Toggle mute
                _playbackService.MuteMedia(!_playbackService.IsMediaMuted());

                // Update the UI
                VolumeIcon = _playbackService.IsMediaMuted() ? "\uE74F" : "\uE767";
            });
            SkipPreviousCommand = new MvxCommand(() => _playbackService.PreviousMedia());
            SkipNextCommand = new MvxCommand(() => _playbackService.NextMedia());
            ToggleMediaStateCommand = new MvxCommand(() =>
            {
                // Get the current state of the media
                var currentState = _playbackService.GetPlaybackState();

                // If the media is currently paused
                if (currentState == PlaybackState.Paused)
                {
                    // Play the media
                    _playbackService.PlayMedia();
                }

                // If the media is currently playing
                if (currentState == PlaybackState.Playing)
                {
                    // Pause the media
                    _playbackService.PauseMedia();
                }
            });
            ToggleTrackLikeCommand = new MvxAsyncCommand(async () =>
            {
                var currentState = IsTrackLiked;
                try
                {
                    if (IsTrackLiked)
                    {
                        IsTrackLiked = false;
                        await _trackService.LikeAsync(CurrentTrack);
                    }
                    else
                    {
                        IsTrackLiked = true;
                        await _trackService.UnlikeAsync(CurrentTrack);
                    }
                }
                catch (Exception e)
                {
                    IsTrackLiked = !currentState;
                }
            });
            MoveMediaPosition = new MvxCommand(() =>
            {
                if (CurrentTrack == null) return;

                if (CurrentTrack.IsLive)
                    return;

                _playbackService.SetMediaPosition(TimeSpan.FromSeconds(CurrentTimeValue));
            });
            CloseCommand = new MvxAsyncCommand(async () => await navigationService.Close(this));

            // Ensure the UI is displayed straight away
            PlaybackService_OnMediaChange(_playbackService.GetCurrentMedia());
            MediaVolume = MediaVolume;

            // Run the timer
            if (!_updateInformationTimer.Enabled)
                _updateInformationTimer.Start();
        }

        /// <summary>
        ///     Timer method that is run to make sure the UI is kept up to date
        /// </summary>
        private async void UpdateInformation(object sender, ElapsedEventArgs e)
        {
            // Only call the following if is media playing and the time is greater then 0.
            if (_playbackService.GetPlaybackState() != PlaybackState.Playing
                || _playbackService.GetMediaPosition().Milliseconds <= 0)
                return;

            if (CurrentTrack == null)
                return;

            // Make sure this runs on the UI thread
            await _mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
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
                    CurrentTimeValue = _playbackService.GetMediaPosition().TotalSeconds;

                    // Get the remaining time for the track
                    var remainingTime = _playbackService.GetMediaDuration().Subtract(_playbackService.GetMediaPosition());

                    // Set the time listened text
                    TimeListened = NumberFormatHelper.FormatTimeString(_playbackService.GetMediaPosition().TotalMilliseconds);

                    // Set the time remaining text
                    TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(remainingTime.TotalMilliseconds);

                    // Set the maximum value
                    MaxTimeValue = _playbackService.GetMediaDuration().TotalSeconds;
                }
            });
        }

        private async void PlaybackService_OnMediaChange(Media media)
        {
            // Make sure this runs on the UI thread
            await _mainThreadDispatcher.ExecuteOnMainThreadAsync(async () =>
            {
                // Update status
                IsShuffleEnabled = _playbackService.IsQueueShuffled();
                IsRepeatEnabled = _playbackService.IsMediaRepeating();

                // Don't run the following if the track did not change
                if (media == CurrentTrack)
                    return;

                if (media is Track t)
                {
                    // Set the new current track, updating the UI
                    CurrentTrack = t;

                    // Update the state
                    if (!t.IsLive)
                    {
                        TimeRemaining = "-" + NumberFormatHelper.FormatTimeString(t.Duration.TotalMilliseconds);
                        TimeListened = "00:00";
                        CurrentTimeValue = 0;
                        MaxTimeValue = t.Duration.TotalSeconds;
                    }
                    else
                    {
                        TimeRemaining = "LIVE";
                        TimeListened = "LIVE";
                        CurrentTimeValue = 1;
                        MaxTimeValue = 1;
                    }

                    // Update the queue
                    UpdateUpNext();

                    // Update the liked status
                    IsTrackLiked = await _trackService.HasLikedAsync(t);
                }
            });
        }

        private async void PlaybackService_OnStateChange(PlaybackState state)
        {
            await _mainThreadDispatcher.ExecuteOnMainThreadAsync(() =>
            {
                switch (state)
                {
                    case PlaybackState.Playing:
                        PlayButtonIcon = PauseIcon;
                        IsLoading = false;
                        break;

                    case PlaybackState.Buffering:
                    case PlaybackState.Opening:
                        PlayButtonIcon = PauseIcon;
                        IsLoading = true;
                        break;

                    default:
                        PlayButtonIcon = PlayIcon;
                        IsLoading = false;
                        break;
                }
            });
        }

        private void UpdateUpNext()
        {
            // Clear
            PreviouslyPlayed.Clear();
            UpNext.Clear();

            // Need track to calculate
            if (CurrentTrack == null) return;

            // Get the queue
            var queue = _playbackService.GetQueue();

            // Items before current
            var beforeCurrent = queue.TakeWhile(x => x != CurrentTrack);
            foreach (var m in beforeCurrent)
            {
                PreviouslyPlayed.Add(m);
            }

            // Items after current
            var afterCurrent = queue.SkipWhile(x => x != CurrentTrack);
            foreach (var m in afterCurrent)
            {
                UpNext.Add(m);
            }
        }
    }
}