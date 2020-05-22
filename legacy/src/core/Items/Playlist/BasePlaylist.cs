using Newtonsoft.Json;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SoundByte.Core.Items.Playlist
{
    /// <summary>
    ///     A universal playlist class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BasePlaylist : BaseItem
    {
        /// <summary>
        ///     What service this playlist belongs to.
        /// </summary>
        [JsonProperty("service_type")]
        public int ServiceType { get; set; }

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Id of the playlist, useful for performing
        ///     tasks on the playlist.
        /// </summary>
        [JsonProperty("playlist_id")]
        public string PlaylistId { get; set; }

        /// <summary>
        ///     Link to the playlist
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; } = "https://soundbytemedia.com/pages/open-default-link";

        /// <summary>
        /// The length of the playlist (all tracks)
        /// </summary>
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Playlist title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Genre of the playlist
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        /// Playlist description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Date the playlist was created.
        /// </summary>
        [JsonProperty("creation_date")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Playlist artwork url
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        /// <summary>
        ///     Playlist thumbnail url
        /// </summary>
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        /// User who created the playlist
        /// </summary>
        [JsonProperty("user")]
        public BaseUser User { get; set; }

        /// <summary>
        ///     Used by SoundByte to determine if the track is in a playlist
        /// </summary>
        [JsonIgnore]
        public bool IsTrackInInternalSet { get; set; }

        /// <summary>
        /// How many likes does this playlist have.
        /// </summary>
        [JsonProperty("likes_count")]
        public double LikesCount { get; set; }

        /// <summary>
        /// How many tracks are in this playlist
        /// </summary>
        [JsonProperty("track_count")]
        public double TrackCount { get; set; }

        /// <summary>
        ///     Custom properties you can set
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();

        #region Methods

        public async Task<(bool success, string errorMessage)> AddTrackAsync(BaseTrack track)
        {
            try
            {
                switch (ServiceType)
                {
                    case ServiceTypes.SoundCloud:
                    case ServiceTypes.SoundCloudV2:
                        var playlistItemSource = new GenericPlaylistItemSource
                        {
                            PlaylistId = PlaylistId,
                            Service = ServiceType
                        };

                        var tracks = new List<BaseTrack>();
                        var playlistToken = string.Empty;
                        var json = string.Empty;

                        // Loop through all tracks
                        while (true)
                        {
                            var items = await playlistItemSource.GetItemsAsync(50, playlistToken);

                            // Add items if successful
                            if (items.IsSuccess)
                                tracks.AddRange(items.Items.Select(x => x.Track));

                            playlistToken = items.Token;
                            if (playlistToken == "eol" || string.IsNullOrEmpty(playlistToken))
                                break;
                        }

                        // If there are tracks in the list
                        if (tracks.Any())
                        {
                            // Start creating the json track string with the basic json
                            json = tracks.Aggregate("{\"playlist\":{\"tracks\":[",
                                (current, t) => current + "{\"id\":\"" + t.TrackId + "\"},");

                            // Complete the json string by adding the current track
                            json += "{\"id\":\"" + track?.TrackId + "\"}]}}";
                        }
                        else
                        {
                            // Only add the one track
                            json = "{\"playlist\":{\"tracks\":[{\"id\":\"" + track?.TrackId + "\"}]}}";
                        }

                        // Create and return the http result.
                        var response = await SoundByteService.Current.PutAsync(ServiceTypes.SoundCloud, $"/playlists/{PlaylistId}/?secret-token={CustomProperties["SecretToken"]}", json);
                        return (response.Response, "Could not add track to playlist");

                    case ServiceTypes.YouTube:

                        // Generate YouTube json for playlist creation
                        var ytJson = "{\"snippet\":{\"playlistId\":\"" + PlaylistId + "\",\"resourceId\":{\"kind\":\"youtube#video\",\"videoId\":\"" + track.TrackId + "\"}}}";
                        var ytItem = (await SoundByteService.Current.PostAsync<YouTubeTrack>(ServiceTypes.YouTube, "/playlistItems", ytJson, new Dictionary<string, string>

                        {
                            { "part", "snippet" },
                            { "onBehalfOfContentOwner", "" }
                        }))?.Response;

                        return (ytItem != null, "Could not add track to playlist");

                    default:
                        return (false, "This service type is not supported: " + ServiceType);
                }
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string errorMessage)> RemoveTrackAsync(BaseTrack track)
        {
            switch (ServiceType)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    var playlistItemSource = new GenericPlaylistItemSource
                    {
                        PlaylistId = PlaylistId,
                        Service = ServiceType
                    };

                    var tracks = new List<BaseTrack>();
                    var playlistToken = string.Empty;

                    // Loop through all tracks
                    while (true)
                    {
                        var items = await playlistItemSource.GetItemsAsync(50, playlistToken);

                        // Add items if successful
                        if (items.IsSuccess)
                            tracks.AddRange(items.Items.Select(x => x.Track));

                        playlistToken = items.Token;
                        if (playlistToken == "eol" || string.IsNullOrEmpty(playlistToken))
                            break;
                    }

                    // Remove this track
                    tracks.Remove(tracks.FirstOrDefault(x => x.TrackId == track.TrackId));

                    // Start creating the json track string with the basic json
                    var json = tracks.Aggregate("{\"playlist\":{\"tracks\":[",
                        (current, t) => current + "{\"id\":\"" + t.TrackId + "\"},");

                    // Complete the json string
                    json = json.TrimEnd(',') + "]}}";

                    // Create and return the http result.
                    var response = await SoundByteService.Current.PutAsync(ServiceTypes.SoundCloud, $"/playlists/{PlaylistId}/?secret-token={CustomProperties["SecretToken"]}", json);
                    return (response.Response, "Could not remove track from playlist");

                case ServiceTypes.YouTube:
                    var youTubePlaylistItemSource = new GenericPlaylistItemSource
                    {
                        PlaylistId = PlaylistId,
                        Service = ServiceType
                    };

                    // Get playlist items and remove current track
                    var ytItems = (await youTubePlaylistItemSource.GetItemsAsync(50, string.Empty)).Items.ToList();
                    var playlistId = ytItems.FirstOrDefault(x => x.Track.TrackId == track.TrackId)?.Track?.CustomProperties["YouTubePlaylistItemId"].ToString();

                    // Delete the item
                    var result = await SoundByteService.Current.DeleteAsync(ServiceTypes.YouTube, "/playlistItems", new Dictionary<string, string>
                    {
                        { "id", playlistId },
                        { "onBehalfOfContentOwner", "" }
                    });

                    return (result.Response, "Could not remove track from playlist");

                default:
                    return (false, "This service type is not supported: " + ServiceType);
            }
        }

        #endregion Methods

        #region Static Methods

        public static async Task<BasePlaylist> CreatePlaylistAsync(int service, string playlistName, bool isPrivate)
        {
            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    // Generate SoundCloud json for playlist creation
                    var scJson = "{\"playlist\":{\"title\":\"" + playlistName.Trim() + "\",\"tracks\":[],\"sharing\":\"" + (isPrivate ? "private" : "public") + "\"}}";
                    return (await SoundByteService.Current.PostAsync<SoundCloudPlaylist>(ServiceTypes.SoundCloud, "/playlists", scJson))?.Response?.ToBasePlaylist();

                case ServiceTypes.YouTube:
                    // Generate YouTube json for playlist creation
                    var ytJson = "{\"snippet\":{\"title\":\"" + playlistName.Trim() + "\"},\"status\":{\"privacyStatus\":\"" + (isPrivate ? "private" : "public") + "\"}}";
                    return (await SoundByteService.Current.PostAsync<YouTubePlaylist>(ServiceTypes.YouTube, "/playlists", ytJson, new Dictionary<string, string>
                    {
                        { "part", "snippet,status" },
                        { "onBehalfOfContentOwner", "" }
                    }))?.Response?.ToBasePlaylist();

                default:
                    throw new Exception("Playlist creation is not supported for " + service);
            }
        }

        public static async Task<BasePlaylist> GetPlaylistAsync(int service, string playlistId)
        {
            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    return (await SoundByteService.Current.GetAsync<SoundCloudPlaylist>(ServiceTypes.SoundCloud, $"/playlists/{playlistId}")).Response.ToBasePlaylist();

                case ServiceTypes.YouTube:
                    // Call the YouTube API and get the items
                    var playlists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceTypes.YouTube, "playlists",
                        new Dictionary<string, string>
                        {
                            {"part", "id,snippet,contentDetails"},
                            {"id", playlistId}
                        }).ConfigureAwait(false);

                    return playlists.Response.Playlists.FirstOrDefault().ToBasePlaylist();

                default:
                    return null;
            }
        }

        #endregion Static Methods
    }
}