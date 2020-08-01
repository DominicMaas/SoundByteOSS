using AVFoundation;
using Foundation;
using MediaPlayer;
using SoundByte.Core.Extensions;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Playback;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using CoreFoundation;
using CoreMedia;
using UIKit;
using Xamarin.Essentials;
using Microsoft.AppCenter.Crashes;

namespace SoundByte.App.iOS.Services
{
    public class PlaybackService : IPlaybackService
    {
        private AVPlayer _player;
        private List<Track> _queue;
        private readonly Random _random;
        private int _currentIndex;

        private string _playlistToken;
        private ISource _playlistSource;

        private bool _isRepeating;

        private readonly IMusicProviderService _musicProviderService;
        private readonly ITelemetryService _telemetryService;
        private readonly IDialogService _dialogService;

        public PlaybackService(IMusicProviderService musicProviderService, ITelemetryService telemetryService, IDialogService dialogService)
        {
            // Setup background audio
            AVAudioSession session = AVAudioSession.SharedInstance();
            session.SetCategory(AVAudioSessionCategory.Playback, AVAudioSessionCategoryOptions.AllowAirPlay | AVAudioSessionCategoryOptions.MixWithOthers
                | AVAudioSessionCategoryOptions.AllowBluetooth | AVAudioSessionCategoryOptions.AllowBluetoothA2DP);
            session.SetActive(true);

            // Setup services
            _musicProviderService = musicProviderService;
            _telemetryService = telemetryService;
            _dialogService = dialogService;

            // Setup player
            _player = new AVPlayer();

            // This should handle times a bit better
            _player.AddPeriodicTimeObserver(CMTime.FromSeconds(1, 1), null, time =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateInfoCenter();
                });
            });

            // Create the queue
            _queue = new List<Track>();

            // Setup the random class
            _random = new Random();

            // Setup Command Center
            var commandCenter = MPRemoteCommandCenter.Shared;
            commandCenter.PreviousTrackCommand.Enabled = true;
            commandCenter.PreviousTrackCommand.AddTarget(PrevCommand);

            commandCenter.NextTrackCommand.Enabled = true;
            commandCenter.NextTrackCommand.AddTarget(NextCommand);

            commandCenter.TogglePlayPauseCommand.Enabled = true;
            commandCenter.TogglePlayPauseCommand.AddTarget(ToggleCommand);

            commandCenter.PlayCommand.Enabled = true;
            commandCenter.PlayCommand.AddTarget(PlayCommand);

            commandCenter.PauseCommand.Enabled = true;
            commandCenter.PauseCommand.AddTarget(PauseCommand);

            commandCenter.ChangeRepeatModeCommand.Enabled = true;
            commandCenter.ChangeRepeatModeCommand.AddTarget(ChangeRepeatModeCommand);

            commandCenter.ChangePlaybackPositionCommand.Enabled = true;
            commandCenter.ChangePlaybackPositionCommand.AddTarget((c) =>
            {
                var e = (MPChangePlaybackPositionCommandEvent)c;

                var time = e.PositionTime;
                _player.SeekAsync(CMTime.FromSeconds(time, 1));
                return MPRemoteCommandHandlerStatus.Success;
            });

            // Setup the pinger to run for app lifetime.
            // This is used to make sure session information is correctly
            // kept as long playing tracks may cause telemetry services to think
            // the app is closed.
            var pingTimer = new Timer { Interval = 180000 };
            pingTimer.Elapsed += (sender, args) => _telemetryService.TrackEvent("Ping");
            pingTimer.Start();
        }

        public event PlaybackServiceEventHandlers.MediaChangedEventHandler OnMediaChange;

        public event PlaybackServiceEventHandlers.StateChangedEventHandler OnStateChange;

        public MPRemoteCommandHandlerStatus PrevCommand(MPRemoteCommandEvent commandEvent)
        {
            PreviousMedia();
            return MPRemoteCommandHandlerStatus.Success;
        }

        public MPRemoteCommandHandlerStatus NextCommand(MPRemoteCommandEvent commandEvent)
        {
            NextMedia();
            return MPRemoteCommandHandlerStatus.Success;
        }

        public MPRemoteCommandHandlerStatus PlayCommand(MPRemoteCommandEvent commandEvent)
        {
            PlayMedia();
            return MPRemoteCommandHandlerStatus.Success;
        }

        public MPRemoteCommandHandlerStatus PauseCommand(MPRemoteCommandEvent commandEvent)
        {
            PauseMedia();
            return MPRemoteCommandHandlerStatus.Success;
        }

        public MPRemoteCommandHandlerStatus ChangeRepeatModeCommand(MPRemoteCommandEvent commandEvent)
        {
            RepeatMedia(!IsMediaRepeating());
            return MPRemoteCommandHandlerStatus.Success;
        }

        public MPRemoteCommandHandlerStatus ToggleCommand(MPRemoteCommandEvent commandEvent)
        {
            if (_player != null)
            {
                if (_player.Rate == 0)
                {
                    PlayMedia();
                }
                else
                {
                    PauseMedia();
                }
            }
            return MPRemoteCommandHandlerStatus.Success;
        }

        public Media GetCurrentMedia()
        {
            return _queue[_currentIndex];
        }

        public TimeSpan GetMediaDuration() => TimeSpan.FromSeconds(_player.CurrentItem.Asset.Duration.Seconds);

        public TimeSpan GetMediaPosition() => TimeSpan.FromSeconds(_player.CurrentTime.Seconds);

        public double GetMediaVolume() => 1.0; // TODO

        public PlaybackState GetPlaybackState()
        {
            return _player.TimeControlStatus switch
            {
                AVPlayerTimeControlStatus.Paused => PlaybackState.Paused,
                AVPlayerTimeControlStatus.WaitingToPlayAtSpecifiedRate => PlaybackState.Buffering,
                AVPlayerTimeControlStatus.Playing => PlaybackState.Playing,
                _ => PlaybackState.None,
            };
        }

        public List<Track> GetQueue() => _queue;

        public ISource GetSource() => _playlistSource;

        public string GetToken() => _playlistToken;

        public async Task<PlaybackInitializeResponse> InitializeAsync(ISource model, IEnumerable<Media> queue = null, string token = null)
        {
            _playlistSource = model;
            _playlistToken = token;

            // We are changing media
            //_mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Pause the media player and clear the current playlist
            _player.Pause();
            _queue.Clear();

            // If the playlist does not exist, or was not passed in, we
            // need to load the first 50 items.
            if (queue == null)
            {
                try
                {
                    // Get (up to) 50 items and update the token
                    var responseItems = await Task.Run(() => _playlistSource.GetItemsAsync(50, _playlistToken));
                    _playlistToken = responseItems.NextToken;
                    queue = responseItems.Items;
                }
                catch (Exception e)
                {
                    return new PlaybackInitializeResponse(false, "Error Loading Playlist: " + e.Message);
                }
            }

            // Loop through all the tracks and add them to the playlist
            foreach (var media in queue)
            {
                if (media is Track track)
                {
                    _queue.Add(track);
                }
            }

            // Everything loaded fine
            return new PlaybackInitializeResponse();
        }

        public bool IsMediaMuted() => _player.Muted;

        public void SetMediaPlaybackRate(double value)
        {
            _dialogService.ShowErrorMessageAsync("Not Supported", "Setting the playback rate is not yet supported.");

            // TODO
        }

        public bool IsMediaRepeating() => _isRepeating;

        public bool IsQueueShuffled()
        {
            return false; // TODO
        }

        public async void MoveToMedia(Media media)
        {
            var pos = _queue.IndexOf((Track)media);
            await MoveToAsync(pos);
        }

        public void MuteMedia(bool mute)
        {
            _player.Muted = mute;
        }

        public void NextMedia()
        {
            if (_queue.Count > 0)
            {
                var pos = (_currentIndex + 1) % _queue.Count;
                MoveToAsync(pos).Forget();
            }
            UpdateInfoCenter();
        }

        public void PauseMedia()
        {
            OnStateChange?.Invoke(PlaybackState.Paused);
            _player?.Pause();
            UpdateInfoCenter();
        }

        public void PlayMedia()
        {
            OnStateChange?.Invoke(PlaybackState.Playing);
            _player?.Play();
            UpdateInfoCenter();
        }

        public void PreviousMedia()
        {
            if (_queue.Count > 0)
            {
                var pos = _currentIndex - 1;
                if (pos < 0)
                {
                    pos = _queue.Count > 0 ? _queue.Count - 1 : 0;
                }
                MoveToAsync(pos).Forget();
            }

            UpdateInfoCenter();
        }

        public void RepeatMedia(bool repeat) => _isRepeating = repeat;

        public void SetMediaPosition(TimeSpan value)
        {
            _player.SeekAsync(CMTime.FromSeconds(value.TotalSeconds, 1));
        }

        public double GetMediaPlaybackRate() => _player.Rate;

        public void SetMediaVolume(double volume) => _player.Volume = (float)volume;

        public Task ShuffleQueueAsync(bool shuffle)
        {
            _dialogService.ShowErrorMessageAsync("Not Supported", "Shuffling is not yet supported");
            return Task.CompletedTask;
        }

        public async Task StartMediaAsync(Media? mediaToPlay, TimeSpan? startTime = null)
        {
            if (mediaToPlay is Track t)
            {
                // find the index of the track in the playlist
                var index = _queue.FindIndex(item => item.TrackId == t.TrackId);
                await MoveToAsync(index);
            }
        }

        public Task StartRandomMediaAsync()
        {
            _dialogService.ShowErrorMessageAsync("Not Supported", "Shuffling is not yet supported");
            return Task.CompletedTask;
        }

        private async Task MoveToAsync(int index)
        {
            PauseMedia();

            _currentIndex = index;
            var url = await GetMediaUrlAsync(_queue[_currentIndex]);
            if (!string.IsNullOrEmpty(url))
            {
                _player.ReplaceCurrentItemWithPlayerItem(new AVPlayerItem(NSUrl.FromString(url)));
                _player.AutomaticallyWaitsToMinimizeStalling = false;

                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, OnComplete);
                OnMediaChange?.Invoke(_queue[_currentIndex]);

                PlayMedia();
            }
            else
            {
                // The url was not found or could not be decoded, go to the next item
                NextMedia();
            }
        }

        private async void OnComplete(NSNotification notif)
        {
            if (_isRepeating)
            {
                await MoveToAsync(_currentIndex);
            }
            else
            {
                NextMedia();
            }
        }

        private void UpdateInfoCenter()
        {
            // The queue must exist, have items, have a current index and the current index must be in range
            if (_queue != null && _queue.Count > 0 && _currentIndex != -1 && _currentIndex < _queue.Count)
            {
                var image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(_queue[_currentIndex].ArtworkUrl)));

                var duration = _queue[_currentIndex].Duration.TotalSeconds == 0
                    ? _player.CurrentItem?.Duration.Seconds ?? 0
                    : _queue[_currentIndex].Duration.TotalSeconds;

                var playInfo = new MPNowPlayingInfo
                {
                    Title = _queue[_currentIndex].Title,
                    Artist = _queue[_currentIndex].User.Username,
                    ElapsedPlaybackTime = _player.CurrentTime.Seconds,
                    PlaybackDuration = duration,
                    PlaybackQueueIndex = _currentIndex,
                    PlaybackQueueCount = _queue.Count,
                    PlaybackRate = _player.Rate,
                    MediaType = MPNowPlayingInfoMediaType.Audio,
                    Artwork = new MPMediaItemArtwork(image)
                };

                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = playInfo;
            }
            else
            {
                MPNowPlayingInfoCenter.DefaultCenter.NowPlaying = null;
            }

            UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
        }

        private async Task<string> GetMediaUrlAsync(Track track)
        {
            try
            {
                var musicProvider = _musicProviderService.MusicProviders.FirstOrDefault(x => x.Identifier == track.MusicProviderId);
                if (musicProvider == null)
                    return null;

                // Call the music provider for the media url
                return await Task.Run(() => musicProvider.CallFunction<string>(musicProvider.Manifest.Playback.OnMusicRequest, track.TrackId));
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return null;
            }
        }
    }
}