using GalaSoft.MvvmLight.Ioc;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.Helpers;
using SoundByte.Core;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using SoundByte.Core.Sources;
using SoundByte.Core.Sources.Generic;
using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.Core.Sources.YouTube;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace SoundByte.App.Uwp.Helpers
{
    /// <summary>
    ///     Helpers for working with and generating protocol items.
    /// </summary>
    public static class ProtocolHelper
    {
        #region Handle Protocol

        /// <summary>
        ///     Handle app protocol. If true is returned this method handled page
        ///     navigation. If false is returned, you must handle app protocol
        /// </summary>
        public static async Task<bool> HandleProtocolAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            try
            {
                var parser = DeepLinkParser.Create(path);
                var section = parser.Root?.Split('/')[0]?.ToLower();

                // Try get the session ID is one was passed through.
                parser.TryGetValue("session", out var sessionId);
                if (!string.IsNullOrEmpty(sessionId))
                    SettingsService.Instance.SessionId = sessionId;

                switch (section)
                {
                    case "resume-playback":
                        await HandleResumeAsync();
                        break;

                    case "cortana":
                        if (!parser.TryGetValue("command", out var command))
                            throw new SoundByteException("Incorrect Protocol", "Command was not supplied (command={command}).");

                        await HandleCortanaCommandAsync(command);
                        break;

                    case "track":
                        if (!parser.TryGetValue("d", out var rawProtocolData))
                            throw new SoundByteException("Incorrect Protocol", "Data was not supplied (d={data}).");

                        await HandleTrackProtocolAsync(rawProtocolData);
                        break;

                    case "user":
                        parser.TryGetValue("id", out var userId);
                        parser.TryGetValue("service", out var userService);

                        // Get user
                        var user = await BaseUser.GetUserAsync(int.Parse(userService), userId);
                        if (user == null)
                            throw new Exception("User does not exist");

                        App.NavigateTo(typeof(UserView), user);
                        return true;

                    case "playlist":
                        parser.TryGetValue("id", out var playlistId);
                        parser.TryGetValue("service", out var playlistService);

                        // Get playlist
                        var playlist = await BasePlaylist.GetPlaylistAsync(int.Parse(playlistService), playlistId);
                        if (playlist == null)
                            throw new Exception("Playlist does not exist");

                        App.NavigateTo(typeof(PlaylistView), playlist);
                        return true;

                    case "playback":
                        parser.TryGetValue("command", out var playbackCommand);
                        HandlePlaybackCommand(playbackCommand);
                        break;
                }

                return false;
            }
            catch (Exception e)
            {
                await NavigationService.Current.CallMessageDialogAsync("The specified protocol is not correct. App will now launch as normal.\n\n" + e.Message);
                return false;
            }
        }

        #endregion Handle Protocol

        #region Playback

        private static void HandlePlaybackCommand(string command)
        {
            switch (command)
            {
                case "play":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().PlayTrack();
                    break;

                case "pause":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().PauseTrack();
                    break;

                case "next":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().NextTrack();
                    break;

                case "previous":
                    SimpleIoc.Default.GetInstance<IPlaybackService>().PreviousTrack();
                    break;
            }
        }

        #endregion Playback

        #region Resume

        public static async Task HandleResumeAsync()
        {
            string playbackData;

            try
            {
                var roamingFolder = ApplicationData.Current.RoamingFolder;
                var playbackFile = await roamingFolder.GetFileAsync("currentPlayback.txt");
                var playbackRawData = await FileIO.ReadTextAsync(playbackFile);

                // Split the data and session information
                if (playbackRawData.Contains('\n'))
                {
                    playbackData = playbackRawData.Split('\n')[0];
                }
                else
                {
                    playbackData = playbackRawData;
                }

                await HandleTrackProtocolAsync(playbackData);
            }
            catch
            {
                return;
            }
        }

        #endregion Resume

        #region Track Protocol

        public static async Task HandleTrackProtocolAsync(string data)
        {
            var protocolData = await DecodeTrackProtocolItemAsync(data);

            await HandleTrackProtocolAsync(protocolData);
        }

        public static async Task HandleTrackProtocolAsync(TrackProtocolItem protocolData)
        {
            // If no source was supplied, use the dummy source
            if (protocolData.Source == null)
                protocolData.Source = new DummyTrackSource();

            // Try init the playlist
            var initPlaylist = await SimpleIoc.Default.GetInstance<IPlaybackService>().InitializePlaylistAsync(protocolData.Source, protocolData.Playlist, protocolData.PlaylistToken);
            if (initPlaylist.Success == false)
                throw new SoundByteException("Could not start playlist", initPlaylist.Message);

            await SimpleIoc.Default.GetInstance<IPlaybackService>().StartTrackAsync(protocolData.Track?.Track, protocolData.TrackPosition);
        }

        public static async Task<TrackProtocolItem> DecodeTrackProtocolItemAsync(string compressedData)
        {
            // Get the raw data string
            var data = StringHelpers.DecompressString(Uri.UnescapeDataString(compressedData));
            var returnItem = new TrackProtocolItem();

            // Get from url
            var paramCollection = data.Split('&');

            // Get the raw data
            var rawTrack = GetParameterString(paramCollection, "t");
            var rawTrackPos = GetParameterString(paramCollection, "tp");
            var rawSource = GetParameterString(paramCollection, "s");
            var rawSourceData = GetParameterString(paramCollection, "sd");
            var rawPlaylistToken = GetParameterString(paramCollection, "pt");
            var rawPlaylist = GetParameterString(paramCollection, "p");
            var rawShuffled = GetParameterString(paramCollection, "is");

            // Add Track
            if (!string.IsNullOrEmpty(rawTrack))
            {
                var trackService = int.Parse(rawTrack.Split(':')[0]);
                var trackId = rawTrack.Split(':')[1];

                var track = await BaseTrack.GetTrackAsync(trackService, trackId);
                returnItem.Track = track;
            }

            // Add track position
            if (!string.IsNullOrEmpty(rawTrackPos))
                returnItem.TrackPosition = TimeSpan.FromSeconds(int.Parse(rawTrackPos));

            // Add source
            if (!string.IsNullOrEmpty(rawSource))
            {
                if (string.IsNullOrEmpty(rawSourceData))
                {
                    returnItem.Source = App.SourceManager.GetTrackSource(rawSource);
                }
                else
                {
                    var sourceData = rawSourceData.Split(',').ToDictionary(x => x.Split(':')[0], x => (object)x.Split(':')[1]);
                    returnItem.Source = App.SourceManager.GetTrackSource(rawSource, sourceData);
                }
            }

            // Add Playlist token
            if (!string.IsNullOrEmpty(rawPlaylistToken))
                returnItem.PlaylistToken = rawPlaylistToken;

            // Add playlist items
            if (!string.IsNullOrEmpty(rawPlaylist))
            {
                var unsortedPlaylistItems = new List<Tuple<int, string>>();

                // Get the data and parse it
                foreach (var trackPair in rawPlaylist.Split(','))
                {
                    var trackService = int.Parse(trackPair.Split(':')[0]);
                    var trackId = trackPair.Split(':')[1];

                    unsortedPlaylistItems.Add(new Tuple<int, string>(trackService, trackId));
                }

                // Sort into wanted groups
                var soundCloudIds = string.Join(',', unsortedPlaylistItems.Where(x => x.Item1 == ServiceTypes.SoundCloud || x.Item1 == ServiceTypes.SoundCloudV2).Select(x => x.Item2));
                var youTubeIds = string.Join(',', unsortedPlaylistItems.Where(x => x.Item1 == ServiceTypes.YouTube).Select(x => x.Item2));

                // Temp list of tracks to shove all information into
                var tempTracks = new List<BaseSoundByteItem>();

                // SoundCloud
                if (!string.IsNullOrEmpty(soundCloudIds))
                    tempTracks.AddRange(await BaseTrack.GetTracksAsync(ServiceTypes.SoundCloud, soundCloudIds));

                // YouTube
                if (!string.IsNullOrEmpty(youTubeIds))
                    tempTracks.AddRange(await BaseTrack.GetTracksAsync(ServiceTypes.YouTube, youTubeIds));

                // Add this list of tracks into the return object
                var rawSortedIds = unsortedPlaylistItems.Select(x => x.Item2).ToList();
                returnItem.Playlist = tempTracks.OrderBy(x => rawSortedIds.IndexOf(x.Track.TrackId));
            }

            // Shuffle
            if (!string.IsNullOrEmpty(rawShuffled))
                returnItem.IsShuffled = rawShuffled == "t";

            return returnItem;
        }

        /// <summary>
        ///     Encode this item into a valid SoundByte protocol item
        /// </summary>
        public static string EncodeTrackProtocolItem(TrackProtocolItem protocolItem, bool includeFullUri)
        {
            // We will add all paramters to this list
            var parameters = new Dictionary<string, string>();

            // Track
            if (protocolItem.Track?.Track != null)
                parameters.Add("t", $"{(int)protocolItem.Track.Track.ServiceType}:{protocolItem.Track.Track.TrackId}");

            // Track duration
            if (protocolItem.TrackPosition != null)
                parameters.Add("tp", protocolItem.TrackPosition.Value.Seconds.ToString());

            // Source and Source details
            if (protocolItem.Source != null)
            {
                parameters.Add("s", protocolItem.Source.GetType().Name);
                parameters.Add("sd", string.Join(',', protocolItem.Source.GetParameters().Select(x => $"{x.Key}:{x.Value}")));
            }

            // Playlist token
            if (!string.IsNullOrEmpty(protocolItem.PlaylistToken))
                parameters.Add("pt", protocolItem.PlaylistToken);

            // Playlist
            if (protocolItem.Playlist != null && protocolItem.Playlist.Any())
            {
                // If we don't have a token, we do not need to send the tracks
                // unless this is a dummy source. This is because our source can
                // load the first set of items.
                if (!string.IsNullOrEmpty(protocolItem.PlaylistToken) &&
                    protocolItem.Source?.GetType() != typeof(DummyTrackSource))
                {
                    parameters.Add("p", string.Join(',', protocolItem.Playlist.Select(x => $"{(int)x.Track.ServiceType}:{x.Track.TrackId}")));
                }
            }

            // Is shuffled
            if (protocolItem.IsShuffled)
                parameters.Add("is", "t");

            // Join all the data
            var contentUri = parameters
                .Where(param => !string.IsNullOrEmpty(param.Key) && !string.IsNullOrEmpty(param.Value))
                .Aggregate(string.Empty, (current, param) => current + "&" + param.Key + "=" + param.Value);

            return includeFullUri
                ? $"sb://track?d={Uri.EscapeDataString(StringHelpers.CompressString(contentUri.TrimStart('&')))}"
                : $"{Uri.EscapeDataString(StringHelpers.CompressString(contentUri.TrimStart('&')))}";
        }

        /// <summary>
        ///     Track protocol data
        /// </summary>
        public class TrackProtocolItem
        {
            public TrackProtocolItem(ISource source = null, BaseSoundByteItem track = null, IEnumerable<BaseSoundByteItem> playlist = null, string playlistToken = null, TimeSpan? trackPosition = null, bool isShuffled = false)
            {
                Track = track;
                Playlist = playlist;
                PlaylistToken = playlistToken;
                Source = source;
                TrackPosition = trackPosition;
                IsShuffled = isShuffled;
            }

            /// <summary>
            ///     The current playing track
            /// </summary>
            public BaseSoundByteItem Track { get; set; }

            /// <summary>
            ///     At what duration is this track
            /// </summary>
            public TimeSpan? TrackPosition { get; set; }

            /// <summary>
            ///     List of tracks in the playlist
            /// </summary>
            public IEnumerable<BaseSoundByteItem> Playlist { get; set; }

            /// <summary>
            ///     Next playlist token to load more items
            /// </summary>
            public string PlaylistToken { get; set; }

            /// <summary>
            ///     The source that this playlist uses
            /// </summary>
            public ISource Source { get; set; }

            /// <summary>
            ///     Is the content shuffled
            /// </summary>
            public bool IsShuffled { get; set; }
        }

        #endregion Track Protocol

        #region Cortana Protocol

        public static async Task HandleCortanaCommandAsync(string command)
        {
            switch (command)
            {
                case "playSoundCloudLikes":
                    if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                    {
                        var scLikeSource = new SoundCloudUserLikeSource
                        {
                            UserId = SoundByteService.Current.GetConnectedUser(ServiceTypes.SoundCloud)?.UserId
                        };

                        await HandleTrackProtocolAsync(new TrackProtocolItem(scLikeSource));
                    }
                    break;

                case "playYouTubeLikes":
                    if (SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube))
                    {
                        await HandleTrackProtocolAsync(new TrackProtocolItem(new YouTubeLikeSource()));
                    }
                    break;

                case "playSoundCloudStream":
                    if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                    {
                        await HandleTrackProtocolAsync(new TrackProtocolItem(new SoundCloudTrackStreamSource()));
                    }
                    break;
            }
        }

        #endregion Cortana Protocol

        #region Helpers

        private static string GetParameterString(string[] parameters, string key)
        {
            var item = parameters.Where(x => x.Contains("=")).FirstOrDefault(x => x.Split('=')[0] == key);

            if (string.IsNullOrEmpty(item))
                return item;

            return item.Split('=')[1];
        }

        #endregion Helpers
    }
}