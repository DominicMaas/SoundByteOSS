#nullable enable

using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Playback;
using SoundByte.Core.Services.Definitions;
using SoundByte.Core.Sources;
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
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Notifications;

namespace SoundByte.App.UWP.Services
{
    public class PlaybackService : IPlaybackService
    {
        private readonly IMusicProviderService _musicProviderService;
        private readonly ITelemetryService _telemetryService;
        private readonly IHistoryService _historyService;
        private readonly IActivityService _activityService;

        private string _playlistToken;
        private ISource _playlistSource;

        private readonly Random _random;

        private readonly MediaPlayer _mediaPlayer;
        private readonly MediaPlaybackList _mediaPlaybackList;

        public PlaybackService(IMusicProviderService musicProviderService, ITelemetryService telemetryService, IHistoryService historyService, IActivityService activityService)
        {
            // Setup services
            _musicProviderService = musicProviderService;
            _telemetryService = telemetryService;
            _historyService = historyService;
            _activityService = activityService;

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

            // Set the last playback volume TODO
            // SetTrackVolume(SettingsService.Instance.PlaybackVolume / 100);

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

        #region UWP Event Handlers

        private void MediaPlayerOnCurrentStateChanged(MediaPlayer sender, object args)
        {
            var state = sender.PlaybackSession.PlaybackState switch
            {
                MediaPlaybackState.None => PlaybackState.None,
                MediaPlaybackState.Opening => PlaybackState.Opening,
                MediaPlaybackState.Buffering => PlaybackState.Buffering,
                MediaPlaybackState.Playing => PlaybackState.Playing,
                MediaPlaybackState.Paused => PlaybackState.Paused,
                _ => throw new Exception("Unknown State Change")
            };

            OnStateChange?.Invoke(state);
        }

        #endregion UWP Event Handlers

        #region Events

        public event PlaybackServiceEventHandlers.MediaChangedEventHandler OnMediaChange;

        public event PlaybackServiceEventHandlers.StateChangedEventHandler OnStateChange;

        #endregion Events

        public Media GetCurrentMedia() => AsMedia(_mediaPlaybackList?.CurrentItem?.Source);

        public TimeSpan GetMediaDuration() => _mediaPlayer.PlaybackSession.NaturalDuration;

        public TimeSpan GetMediaPosition() => _mediaPlayer.PlaybackSession.Position;

        public double GetMediaVolume() => _mediaPlayer.Volume;

        public PlaybackState GetPlaybackState()
        {
            return _mediaPlayer.PlaybackSession.PlaybackState switch
            {
                MediaPlaybackState.None => PlaybackState.None,
                MediaPlaybackState.Opening => PlaybackState.Opening,
                MediaPlaybackState.Buffering => PlaybackState.Buffering,
                MediaPlaybackState.Playing => PlaybackState.Playing,
                MediaPlaybackState.Paused => PlaybackState.Paused,
                _ => PlaybackState.None,
            };
        }

        public List<Track> GetQueue()
        {
            return IsQueueShuffled()
                ? _mediaPlaybackList.ShuffledItems.Select(x => AsMedia(x.Source)).Where(y => y.MediaType == MediaType.Track).Cast<Track>().ToList()
                : _mediaPlaybackList.Items.Select(x => AsMedia(x.Source)).Where(y => y.MediaType == MediaType.Track).Cast<Track>().ToList();
        }

        public ISource GetSource() => _playlistSource;

        public string GetToken() => _playlistToken;

        public async Task<PlaybackInitializeResponse> InitializeAsync(ISource model, IEnumerable<Media> queue = null, string token = null)
        {
            _playlistSource = model;
            _playlistToken = token;

            // We are changing media
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;

            // Pause the media player and clear the current playlist
            _mediaPlayer.Pause();
            _mediaPlaybackList.Items.Clear();

            // If the queue does not exist, or was not passed in, we
            // need to load the first 50 items.
            if (queue == null)
            {
                try
                {
                    // Get (up to) 50 items and update the token
                    var responseItems = await _playlistSource.GetItemsAsync(25, _playlistToken);
                    _playlistToken = responseItems.NextToken;
                    queue = responseItems.Items;
                }
                catch (Exception e)
                {
                    return new PlaybackInitializeResponse(false, "Error Loading Playlist: " + e.Message);
                }
            }

            // Loop through all the media and add them to the playlist
            foreach (var media in queue)
            {
                if (!(media.MediaType == MediaType.Track || media.MediaType == MediaType.PodcastEpisode))
                    continue;

                try
                {
                    BuildMediaItem(media);
                }
                catch (Exception e)
                {
                    _telemetryService.TrackEvent("Playback Item Addition Failed", new Dictionary<string, string>
                    {
                        { "MediaType", media.MediaType.ToString() },
                        { "MediaService", media.MusicProviderId.ToString() },
                        { "ErrorMessage", e.Message }
                    });
                }
            }

            // Everything loaded fine
            return new PlaybackInitializeResponse();
        }

        public bool IsMediaMuted() => _mediaPlayer.IsMuted;

        public void SetMediaPlaybackRate(double value) => _mediaPlayer.PlaybackSession.PlaybackRate = value;

        public bool IsMediaRepeating() => _mediaPlayer.IsLoopingEnabled;

        public bool IsQueueShuffled() => _mediaPlaybackList.ShuffleEnabled;

        public void MoveToMedia(Media media)
        {
            // Find the index of the track in the playlist
            var index = _mediaPlaybackList.Items.ToList().FindIndex(item => AsMedia(item.Source) == media);

            if (index == -1)
                return;

            // Move to the track
            _mediaPlaybackList.MoveTo((uint)index);
        }

        public void MuteMedia(bool mute) => _mediaPlayer.IsMuted = mute;

        public void NextMedia()
        {
            _mediaPlayer.SystemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Changing;
            _mediaPlaybackList.MoveNext();
        }

        public void PauseMedia()
        {
            if (_mediaPlayer.PlaybackSession.CanPause)
                _mediaPlayer.Pause();
        }

        public void PlayMedia()
        {
            _mediaPlayer.Play();
        }

        public void PreviousMedia()
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

        public void RepeatMedia(bool repeat) => _mediaPlayer.IsLoopingEnabled = repeat;

        public void SetMediaPosition(TimeSpan value) => _mediaPlayer.PlaybackSession.Position = value;

        public double GetMediaPlaybackRate() => _mediaPlayer.PlaybackSession.PlaybackRate;

        public void SetMediaVolume(double volume) => _mediaPlayer.Volume = volume;

        public async Task ShuffleQueueAsync(bool shuffle)
        {
            if (shuffle)
            {
                // Start a random track
                await StartRandomMediaAsync().ConfigureAwait(false);
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

        public async Task StartMediaAsync(Media mediaToPlay, TimeSpan? startTime = null)
        {
            _mediaPlaybackList.ShuffleEnabled = false;

            _mediaPlayer.Pause();

            if (mediaToPlay == null)
            {
                try
                {
                    // await App.RoamingService.StopActivityAsync();
                    // await App.RoamingService.StartActivityAsync(_playlistSource, _mediaPlaybackList.Items.FirstOrDefault()?.Source?.AsBaseTrack(),
                    //     _mediaPlaybackList.Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack())), _playlistToken, null, IsPlaylistShuffled());
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
                    // Find the index of the track in the playlist
                    var index = _mediaPlaybackList.Items.ToList()
                        .FindIndex(item => AsMedia(item.Source) == mediaToPlay);

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

                    //await App.RoamingService.StopActivityAsync();
                    // await App.RoamingService.StartActivityAsync(_playlistSource, trackToPlay, _mediaPlaybackList.Items.Select(x => new BaseSoundByteItem(x.Source.AsBaseTrack())),
                    //    _playlistToken, null, IsPlaylistShuffled());

                    return;
                }
                catch (Exception ex)
                {
                    keepTrying++;
                    await Task.Delay(150);
                }
            }

            // Just play the first item
            _mediaPlayer.Play();
        }

        public async Task StartRandomMediaAsync()
        {
            // The amount of items to play.
            var playItemsCount = _mediaPlaybackList.Items.Count;

            // Get a random index
            var index = _random.Next(0, playItemsCount - 1);

            // Start the track
            await StartMediaAsync(AsMedia(_mediaPlaybackList.Items[index]?.Source)).ConfigureAwait(false);

            // Call this afterwards (as the above method sorts items)
            _mediaPlaybackList.ShuffleEnabled = true;
        }

        #region Private Methods

        /// <summary>
        ///     Occurs when a current media playback item changes.
        /// </summary>
        private async void MediaPlaybackListOnCurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            // If there is no new item, don't do anything
            if (args.NewItem == null) return;

            var media = AsMedia(args.NewItem.Source);

            // Invoke the media change method
            OnMediaChange?.Invoke(media);

            // Update the live tile
            UpdateTile(media);

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

                // Analytics
                _telemetryService.TrackEvent("Current Song Change", new Dictionary<string, string>
                {
                    { "Current Usage", currentUsageLimit },
                    { "Free", SystemInformation.AvailableMemory.ToString(CultureInfo.InvariantCulture) },
                    { "Type", media.MediaType.ToString() },
                    { "Provider", media.MusicProviderId.ToString() },
                    { "Device", SystemInformation.DeviceFamily },
                    { "Current Version / First Version", SystemInformation.FirstVersionInstalled.ToFormattedString() + "/" + SystemInformation.ApplicationVersion.ToFormattedString()},
                });

                // History
                try
                {
                    await _historyService.AddToHistoryAsync(media);
                }
                catch (Exception ex)
                {
                    _telemetryService.TrackException(ex);
                }
            });

            try
            {
                // The correct playlist
                var tempPlaylist = new List<Media>();
                tempPlaylist.AddRange(_mediaPlaybackList.ShuffleEnabled
                    ? _mediaPlaybackList.ShuffledItems.Select(x => AsMedia(x.Source)).ToList()
                   : _mediaPlaybackList.Items.Select(x => AsMedia(x.Source)).ToList());

                // Update roaming activity
                await _activityService.UpdateActivityAsync(_playlistSource, media, tempPlaylist, _playlistToken, null, IsQueueShuffled());
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
                    var newItems = await _playlistSource.GetItemsAsync(25, _playlistToken);
                    _playlistToken = newItems.NextToken;

                    if (newItems.Successful)
                    {
                        // Loop through all the tracks and add them to the playlist
                        foreach (var newMedia in newItems.Items)
                        {
                            try
                            {
                                BuildMediaItem(newMedia);
                            }
                            catch (Exception e)
                            {
                                _telemetryService.TrackEvent("Playback Item Addition Failed", new Dictionary<string, string>
                                {
                                    { "MediaType", newMedia.MediaType.ToString() },
                                    { "MusicProvider", newMedia.MusicProviderId.ToString() },
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
        /// <param name="media">Media to build into a media item</param>
        private void BuildMediaItem(Media media)
        {
            if (media.MediaType != MediaType.Track) return;
            var track = (Track)media;

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
            source.CustomProperties["SOUNDBYTE_ITEM"] = media;

            var mediaPlaybackItem = new MediaPlaybackItem(source);

            // Apply display properties to this item
            var displayProperties = mediaPlaybackItem.GetDisplayProperties();
            displayProperties.Type = MediaPlaybackType.Music;
            displayProperties.MusicProperties.Title = track.Title;
            displayProperties.MusicProperties.Artist = track.User.Username;
            displayProperties.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(track.ArtworkUrl));

            // Apply the properties
            mediaPlaybackItem.ApplyDisplayProperties(displayProperties);

            // Add this item to the list
            _mediaPlaybackList.Items.Add(mediaPlaybackItem);
        }

        private async void BindMediaSource(MediaBinder sender, MediaBindingEventArgs args)
        {
            var deferal = args.GetDeferral();

            // Only tracks are supported
            var media = JsonConvert.DeserializeObject<Media>(args.MediaBinder.Token);
            if (media.MediaType != MediaType.Track) return;

            // Get the track data
            var track = JsonConvert.DeserializeObject<Track>(args.MediaBinder.Token);

            try
            {
                // Get the associated music provider
                var musicProvider = _musicProviderService.MusicProviders.FirstOrDefault(x => x.Identifier == track.MusicProviderId);
                if (musicProvider == null) return;

                // Call the music provider for the media url
                var url = await Task.Run(() => musicProvider.CallFunction<string>(musicProvider.Manifest.Playback.OnMusicRequest, track.TrackId));
                if (string.IsNullOrEmpty(url)) return;

                // Set the media url
                var source = await AdaptiveMediaSource.CreateFromUriAsync(new Uri(url));
                if (source.Status == AdaptiveMediaSourceCreationStatus.Success)
                {
                    args.SetAdaptiveMediaSource(source.MediaSource);
                }
                else
                {
                    // Fallback
                    args.SetUri(new Uri(url));
                }
            }
            catch (Exception e)
            {
                // So we know an error has occured
                _telemetryService.TrackEvent("Media Item Load Fail", new Dictionary<string, string> {
                    { "MediaType", track.MediaType.ToString() },
                    { "MusicProvider", track.MusicProviderId.ToString() },
                    {  "ErrorMessage", e.Message }
                });

                // TODO
                //if (!(DeviceHelper.IsBackground || DeviceHelper.IsXbox))
                // {
                //      await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                //     {
                //         App.NotificationManager?.Show("An error occurred while trying to play or preload the track '" + track?.Title + "' (" + track?.ServiceType + ").\n\nError message: " + e.Message, 6500);
                //     });
                // }
            }
            finally
            {
                deferal.Complete();
            }

            // TODO: Local
            // var fileToken = track.CustomProperties["file_token"].ToString();
            // var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(fileToken);

            // args.SetStorageFile(file);
            // break;
        }

        private void UpdateTile(Media media)
        {
            // Not supported on xbox TODO
            //if (DeviceHelper.IsXbox)
            //    return;

            var title = media.MediaType switch
            {
                MediaType.Playlist => ((Playlist)media).Title,
                MediaType.Track => ((Track)media).Title,
                _ => null
            };

            var artworkUrl = media.MediaType switch
            {
                MediaType.Playlist => ((Playlist)media).ArtworkUrl,
                MediaType.Track => ((Track)media).ArtworkUrl,
                _ => null
            };

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
                                Source = artworkUrl,
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
                                    Text = title,
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
                                Source = artworkUrl,
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
                                    Text = title,
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
                                Source = artworkUrl,
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
                                    Text = title,
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
                                Source = artworkUrl,
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
                                    Text = title,
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

        #region Extensions

        private static Media? AsMedia(MediaSource? source)
        {
            return source?.CustomProperties["SOUNDBYTE_ITEM"] as Media;
        }

        #endregion Extensions
    }
}