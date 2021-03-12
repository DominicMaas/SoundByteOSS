using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.App.Uwp.Common;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Notifications;
using YoutubeExplode;

namespace SoundByte.App.Uwp.ServicesV2.Implementations
{
    /// <summary>
    ///     UWP implementation of the Playback Service
    /// </summary>
    public class PlaybackService : IPlaybackService
    {
        #region Private Variables

        private readonly Random _random;

        private string _playlistToken;
        private ISource _playlistSource;

        private readonly YoutubeClient _youTubeClient;
        private readonly MediaPlayer _mediaPlayer;
        private readonly MediaPlaybackList _mediaPlaybackList;

        #endregion Private Variables

        #region Services

        private readonly ITelemetryService _telemetryService;

        #endregion Services

        #region Constructor

        /// <summary>
        /// Setup the playback service class for use.
        /// </summary>
        public PlaybackService(ITelemetryService telemetryService)
        {
            // Setup services
            _telemetryService = telemetryService;

            // Setup the random class
            _random = new Random();

            // Only keep 3 items open and do not auto repeat
            // as we will be loading more items once we reach the
            // end of a list (or starting over if in playlist)
            _mediaPlaybackList = new MediaPlaybackList
            {
                MaxPlayedItemsToKeepOpen = 3,
                AutoRepeatEnabled = true,
            };

            // Create the media player and disable auto play
            // as we are going to use a playback list. Set the
            // source to the media playback list. Auto play is true so if
            // we reach the end of a playlist (or source) start from the beginning.
            _mediaPlayer = new MediaPlayer
            {
                AutoPlay = false,
                Source = _mediaPlaybackList
            };

            // Set the last playback volume
            SetTrackVolume(SettingsService.Instance.PlaybackVolume / 100);

            // Create the YouTube client used to parse YouTube streams.
            _youTubeClient = new YoutubeClient();

            // Assign event handlers
            _mediaPlaybackList.CurrentItemChanged += MediaPlaybackListOnCurrentItemChanged;
            _mediaPlayer.CurrentStateChanged += MediaPlayerOnCurrentStateChanged;

            // Setup the pinger to run for app lifetime.
            // This is used to make sure session information is correctly
            // kept as long playing tracks may cause telemetry services to think
            // the app is closed.
            var pingTimer = new Timer { Interval = 180000 };
            pingTimer.Elapsed += (sender, args) => _telemetryService.TrackEvent("Ping");
            pingTimer.Start();
        }

        private void MediaPlayerOnCurrentStateChanged(MediaPlayer sender, object args)
        {
            OnStateChange?.Invoke(sender.PlaybackSession.PlaybackState);
        }

        #endregion Constructor

        #region Events

        public event IPlaybackServiceEventHandlers.TrackChangedEventHandler OnTrackChange;

        public event IPlaybackServiceEventHandlers.StateChangedEventHandler OnStateChange;

        #endregion Events

        #region Wrappers

        public MediaPlayer GetMediaPlayer() => _mediaPlayer;

        public MediaPlaybackList GetMediaPlaybackList() => _mediaPlaybackList;

        public YoutubeClient GetYoutubeClient() => _youTubeClient;

        #endregion Wrappers

        #region Shuffle Playlist

        public bool IsPlaylistShuffled() =>
             _mediaPlaybackList.ShuffleEnabled;

        public async Task ShufflePlaylistAsync(bool shuffle)
        {
            if (shuffle)
            {
                // Start a random track
                await StartRandomTrackAsync().ConfigureAwait(false);
            }
            else
            {
                // Disable shuffle
                _mediaPlaybackList.ShuffleEnabled = false;
            }

            // Track event
            _telemetryService.TrackEvent("Shuffle Playlist", new Dictionary<string, string>
            {
                { "IsShuffled", shuffle.ToString() }
            });
        }

        #endregion Shuffle Playlist

        #region Track Volume & Mute

        public bool IsTrackMuted()
            => _mediaPlayer.IsMuted;

        public void MuteTrack(bool mute)
            => _mediaPlayer.IsMuted = mute;

        public double GetTrackVolume()
            => _mediaPlayer.Volume;

        public void SetTrackVolume(double volume)
            => _mediaPlayer.Volume = volume;

        #endregion Track Volume & Mute

        #region Track Position

        public TimeSpan GetTrackPosition()
            => _mediaPlayer.PlaybackSession.Position;

        public void SetTrackPosition(TimeSpan value)
            => _mediaPlayer.PlaybackSession.Position = value;

        #endregion Track Position

        #region Track Repeat

        public bool IsTrackRepeating()
            => _mediaPlayer.IsLoopingEnabled;

        public void RepeatTrack(bool repeat)
            => _mediaPlayer.IsLoopingEnabled = repeat;

        #endregion Track Repeat

        #region Track Controls

        public void NextTrack()
        {
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
            _mediaPlaybackList.MoveNext();
        }

        public void PreviousTrack()
        {
            if (_mediaPlayer.PlaybackSession.Position.TotalSeconds <= 10)
            {
                _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
                _mediaPlaybackList.MovePrevious();
            }
            else
            {
                _mediaPlayer.PlaybackSession.Position = TimeSpan.Zero;
            }
        }

        public void PauseTrack()
        {
            if (_mediaPlayer.PlaybackSession.CanPause)
                _mediaPlayer.Pause();
        }

        public void PlayTrack()
        {
            _mediaPlayer.Play();
        }

        public void MoveToTrack(BaseTrack track)
        {
            // Find the index of the track in the playlist
            var index = _mediaPlaybackList.Items.ToList()
                .FindIndex(item => item.Source.AsBaseTrack().TrackId
                                   == track.TrackId);

            if (index == -1)
                return;

            // Move to the track
            _mediaPlaybackList.MoveTo((uint)index);
        }

        #endregion Track Controls

        #region Track Information

        public TimeSpan GetTrackDuration()
            => _mediaPlayer.PlaybackSession.NaturalDuration;

        public MediaPlaybackState GetPlaybackState()
            => _mediaPlayer.PlaybackSession.PlaybackState;

        public BaseTrack GetCurrentTrack()
            => _mediaPlaybackList?.CurrentItem?.Source?.AsBaseTrack();

        #endregion Track Information

        #region Playlist Information

        public List<BaseTrack> GetPlaylist()
        {
            return IsPlaylistShuffled()
                ? _mediaPlaybackList.ShuffledItems.Select(x => x.Source.AsBaseTrack()).ToList()
                : _mediaPlaybackList.Items.Select(x => x.Source.AsBaseTrack()).ToList();
        }

        public string GetPlaylistToken()
            => _playlistToken;

        public ISource GetPlaylistSource()
            => _playlistSource;

        #endregion Playlist Information

        #region Start Track

        public async Task StartTrackAsync(BaseTrack trackToPlay, TimeSpan? startTime = null)
        {
            _mediaPlaybackList.ShuffleEnabled = false;

            _mediaPlayer.Pause();

            if (trackToPlay == null)
            {
                try
                {
                    await App.RoamingService.StopActivityAsync();
                    await App.RoamingService.StartActivityAsync(_playlistSource, _mediaPlaybackList.Items.FirstOrDefault()?.Source?.AsBaseTrack(),
                        _mediaPlaybackList.Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack())), _playlistToken, null, IsPlaylistShuffled());
                }
                catch
                {
                    // Activity failing is not important
                }

                _mediaPlayer.Play();
                return;
            }

            var keepTrying = 0;

            while (keepTrying < 50)
            {
                try
                {
                    // find the index of the track in the playlist
                    var index = _mediaPlaybackList.Items.ToList()
                        .FindIndex(item => item.Source.AsBaseTrack().TrackId
                                           == trackToPlay.TrackId);

                    if (index == -1)
                    {
                        await Task.Delay(50);
                        keepTrying++;
                        continue;
                    }

                    // Move to the track
                    _mediaPlaybackList.MoveTo((uint)index);

                    // Set the position if we supply one
                    if (startTime.HasValue)
                        _mediaPlayer.PlaybackSession.Position = startTime.Value;

                    // Begin playing
                    _mediaPlayer.Play();

                    await App.RoamingService.StopActivityAsync();
                    await App.RoamingService.StartActivityAsync(_playlistSource, trackToPlay, _mediaPlaybackList.Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack())),
                        _playlistToken, null, IsPlaylistShuffled());

                    return;
                }
                catch (Exception)
                {
                    keepTrying++;
                    await Task.Delay(150);
                }
            }

            // Just play the first item
            _mediaPlayer.Play();
        }

        public async Task StartRandomTrackAsync()
        {
            // The amount of items to play.
            var playItemsCount = _mediaPlaybackList.Items.Count;

            // Get a random index
            var index = _random.Next(0, playItemsCount - 1);

            // Start the track
            await StartTrackAsync(_mediaPlaybackList.Items[index]?.Source.AsBaseTrack()).ConfigureAwait(false);

            // Call this afterwards (as the above method unshuffles items)
            _mediaPlaybackList.ShuffleEnabled = true;
        }

        #endregion Start Track

        #region Initialize Playlist

        public async Task<PlaybackInitializeResponse> InitializePlaylistAsync(ISource model, IEnumerable<BaseSoundByteItem> playlist = null, string token = null)
        {
            _playlistSource = model;
            _playlistToken = token;

            // We are changing media
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Pause the media player and clear the current playlist
            _mediaPlayer.Pause();
            _mediaPlaybackList.Items.Clear();

            // If the playlist does not exist, or was not passed in, we
            // need to load the first 50 items.
            if (playlist == null)
            {
                try
                {
                    // Get (up to) 50 items and update the token
                    var responseItems = await _playlistSource.GetItemsAsync(50, _playlistToken);
                    _playlistToken = responseItems.Token;
                    playlist = responseItems.Items;
                }
                catch (Exception e)
                {
                    return new PlaybackInitializeResponse(false, "Error Loading Playlist: " + e.Message);
                }
            }

            // Loop through all the tracks and add them to the playlist
            foreach (var track in playlist)
            {
                if (track.Type != ItemType.Track)
                    continue;

                try
                {
                    BuildMediaItem(track.Track);
                }
                catch (Exception e)
                {
                    _telemetryService.TrackEvent("Playback Item Addition Failed", new Dictionary<string, string>
                    {
                        { "TrackID", track.Track.TrackId },
                        { "TrackService", track.Track.ServiceType.ToString() },
                        { "ErrorMessage", e.Message }
                    });
                }
            }

            // Everything loaded fine
            return new PlaybackInitializeResponse();
        }

        #endregion Initialize Playlist

        #region Private Methods

        /// <summary>
        ///     Occurs when a current media playback item changes.
        /// </summary>
        private void MediaPlaybackListOnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args) =>
            MediaPlaybackListOnCurrentItemChangedAsync(sender, args).FireAndForgetSafeAsync();

        private async Task MediaPlaybackListOnCurrentItemChangedAsync(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            var track = args.NewItem?.Source.AsBaseTrack();

            // If there is no new item, don't do anything
            if (track == null)
                return;

            // Invoke the track change method
            OnTrackChange?.Invoke(track);

            // Update the live tile
            UpdateTile(track);

            await Task.Run(async () =>
            {
                string currentUsageLimit;
                var memoryUsage = MemoryManager.AppMemoryUsage / 1024 / 1024;

                if (memoryUsage > 512)
                {
                    currentUsageLimit = "More than 512MB";
                }
                else if (memoryUsage > 256)
                {
                    currentUsageLimit = "More than 256MB";
                }
                else if (memoryUsage > 128)
                {
                    currentUsageLimit = "More than 128MB";
                }
                else
                {
                    currentUsageLimit = "Less than 128MB";
                }

                _telemetryService.TrackEvent("Current Song Change", new Dictionary<string, string>
                {
                    { "Current Usage", currentUsageLimit },
                    { "Free", SystemInformation.AvailableMemory.ToString(CultureInfo.InvariantCulture) },
                    { "Track Type", track.ServiceType.ToString() ?? "Null" },
                    { "Device", SystemInformation.DeviceFamily },
                    { "Current Version / First Version", SystemInformation.FirstVersionInstalled.ToFormattedString() + "/" + SystemInformation.ApplicationVersion.ToFormattedString()},
                });
            });

            try
            {
                // The correct playlist
                var tempPlaylist = new List<BaseSoundByteItem>();
                tempPlaylist.AddRange(_mediaPlaybackList.ShuffleEnabled
                    ? _mediaPlaybackList.ShuffledItems.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack())).ToList()
                    : _mediaPlaybackList.Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack())).ToList());

                // Update roaming activity
                await App.RoamingService.UpdateActivityAsync(_playlistSource, track, tempPlaylist, _playlistToken, null, IsPlaylistShuffled());

                // Update the resume files
                var roamingFolder = SettingsService.Instance.SyncLastPlayed
                    ? ApplicationData.Current.RoamingFolder
                    : ApplicationData.Current.LocalFolder;

                // Save in file
                var playbackFile = await roamingFolder.CreateFileAsync("currentPlayback.txt", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(playbackFile, ProtocolHelper.EncodeTrackProtocolItem(new ProtocolHelper.TrackProtocolItem(_playlistSource, new BaseSoundByteItem(track), tempPlaylist, _playlistToken, null, _mediaPlaybackList.ShuffleEnabled), false));
            }
            catch
            {
                // Ignore
            }

            // Find the index of this item and see if we are near the end
            var currentIndex = _mediaPlaybackList.ShuffleEnabled
                ? _mediaPlaybackList.ShuffledItems.ToList().IndexOf(args.NewItem)
                : _mediaPlaybackList.Items.IndexOf(args.NewItem);

            var maxIndex = _mediaPlaybackList.ShuffleEnabled
                ? _mediaPlaybackList.ShuffledItems.Count - 1
                : _mediaPlaybackList.Items.Count - 1;

            // When we are three items from the end, load more items
            if (currentIndex >= maxIndex - 3)
            {
                try
                {
                    var newItems = await _playlistSource.GetItemsAsync(50, _playlistToken);
                    _playlistToken = newItems.Token;

                    if (newItems.IsSuccess)
                    {
                        // Loop through all the tracks and add them to the playlist
                        foreach (var newTrack in newItems.Items)
                        {
                            if (newTrack.Type != ItemType.Track)
                                continue;

                            try
                            {
                                BuildMediaItem(newTrack.Track);
                            }
                            catch (Exception e)
                            {
                                _telemetryService.TrackEvent("Playback Item Addition Failed", new Dictionary<string, string>
                                {
                                    { "TrackID", newTrack.Track.TrackId },
                                    { "TrackService", newTrack.Track.ServiceType.ToString() },
                                    { "ErrorMessage", e.Message }
                                });
                            }
                        }
                    }
                }
                catch
                {
                    _playlistToken = "eol";
                }
            }
        }

        /// <summary>
        ///     Build a media item and add it to the list
        /// </summary>
        /// <param name="track">Track to build into a media item</param>
        private void BuildMediaItem(BaseTrack track)
        {
            // Create a media binding for later (this is used to
            // load the track streams as we need them).
            var binder = new MediaBinder
            {
                Token = JsonConvert.SerializeObject(track)
            };
            binder.Binding += BindMediaSource;

            // Create the source, bind track metadata and use it to
            // create a playback item
            var source = MediaSource.CreateFromMediaBinder(binder);
            var mediaPlaybackItem = new MediaPlaybackItem(track.AsMediaSource(source));

            // Apply display properties to this item
            var displayProperties = mediaPlaybackItem.GetDisplayProperties();
            displayProperties.Type = MediaPlaybackType.Music;
            displayProperties.MusicProperties.Title = track.Title;
            displayProperties.MusicProperties.Artist = track.User.Username;
            displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(track.ThumbnailUrl));

            // Apply the properties
            mediaPlaybackItem.ApplyDisplayProperties(displayProperties);

            // Add this item to the list
            _mediaPlaybackList.Items.Add(mediaPlaybackItem);
        }

        private void BindMediaSource(MediaBinder sender, MediaBindingEventArgs args) =>
            BindMediaSourceAsync(sender, args).FireAndForgetSafeAsync();

        private async Task BindMediaSourceAsync(MediaBinder sender, MediaBindingEventArgs args)
        {
            var deferal = args.GetDeferral();

            // Get the track data
            var track = JsonConvert.DeserializeObject<BaseTrack>(args.MediaBinder.Token);

            try
            {
                // Only run if the track exists
                if (track != null)
                {
                    switch (track.ServiceType)
                    {
                        case ServiceTypes.YouTube:
                            var youTubeAudioUrl = await track.GetAudioStreamAsync(_youTubeClient);
                            if (!string.IsNullOrEmpty(youTubeAudioUrl))
                            {
                                if (track.IsLive)
                                {
                                    var source = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(youTubeAudioUrl));
                                    if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                                    {
                                        args.SetAdaptiveMediaSource(source.MediaSource);
                                    }
                                }
                                else
                                {
                                    args.SetUri(new Uri(youTubeAudioUrl));
                                }
                            }
                            break;

                        case ServiceTypes.Local:
                            var fileToken = track.CustomProperties["file_token"].ToString();
                            var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken);

                            args.SetStorageFile(file);
                            break;

                        case ServiceTypes.ITunesPodcast:
                            args.SetUri(new Uri(track.AudioStreamUrl));
                            break;

                        default:
                            // Get the audio stream url for this track
                            var audioStreamUrl = await track.GetAudioStreamAsync(_youTubeClient);
                            if (!string.IsNullOrEmpty(audioStreamUrl))
                            {
                                // Set generic stream url.
                                args.SetUri(new Uri(audioStreamUrl));
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // So we know an error has occured
                _telemetryService.TrackEvent("Media Item Load Fail", new Dictionary<string, string> {
                    { "Message", e.Message },
                    { "Service Type", track.ServiceType.ToString() },
                    { "Item ID", track.TrackId }
                });

                if (!(DeviceHelper.IsBackground || DeviceHelper.IsXbox))
                {
                    await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                    {
                        App.NotificationManager?.Show("An error occurred while trying to play or preload the track '" + track?.Title + "' (" + track?.ServiceType + ").\n\nError message: " + e.Message, 6500);
                    });
                }
            }

            deferal.Complete();
        }

        private void UpdateTile(BaseTrack track)
        {
            // Not supported on xbox
            if (DeviceHelper.IsXbox)
                return;

            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            var content = new TileContent
            {
                Visual = new TileVisual
                {
                    Branding = TileBranding.NameAndLogo,

                    TileMedium = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            PeekImage = new TilePeekImage
                            {
                                Source = track.ArtworkUrl,
                                HintOverlay = 30
                            },

                            Children =
                            {
                                new AdaptiveText
                                {
                                    Text = "Now Playing"
                                },
                                new AdaptiveText
                                {
                                    Text = track.Title,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                    },

                    TileLarge = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            PeekImage = new TilePeekImage
                            {
                                Source = track.ArtworkUrl,
                                HintOverlay = 30
                            },

                            Children =
                            {
                                new AdaptiveText
                                {
                                    Text = "Now Playing"
                                },
                                new AdaptiveText
                                {
                                    Text = track.Title,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                    },

                    TileSmall = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            PeekImage = new TilePeekImage
                            {
                                Source = track.ArtworkUrl,
                                HintOverlay = 30
                            },

                            Children =
                            {
                                new AdaptiveText
                                {
                                    Text = "Now Playing"
                                },
                                new AdaptiveText
                                {
                                    Text = track.Title,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive
                        {
                            PeekImage = new TilePeekImage
                            {
                                Source = track.ArtworkUrl,
                                HintOverlay = 30
                            },

                            Children =
                            {
                                new AdaptiveText
                                {
                                    Text = "Now Playing"
                                },
                                new AdaptiveText
                                {
                                    Text = track.Title,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                    }
                }
            };

            var notification = new TileNotification(content.GetXml());
            updater.Update(notification);
        }

        #endregion Private Methods
    }
}