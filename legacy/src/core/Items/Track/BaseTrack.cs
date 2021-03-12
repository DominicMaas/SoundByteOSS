using Newtonsoft.Json;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace SoundByte.Core.Items.Track
{
    /// <summary>
    ///     A universal track class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BaseTrack : BaseItem
    {
        /// <summary>
        ///     Get the audio stream for this item
        /// </summary>
        /// <param name="youTubeClient">Used for YouTube videos</param>
        /// <returns></returns>
        public async Task<string> GetAudioStreamAsync(YoutubeClient youTubeClient)
        {
            // Get the appropriate client Ids
            var service = SoundByteService.Current.Services.FirstOrDefault(x => x.Service == ServiceType);

            if (service == null)
                throw new Exception("Oh shit, this should like, never be null dude. You should probably direct message me on twitter :D (@dominicjmaas)");

            string audioStream;

            switch (ServiceType)
            {
                case ServiceTypes.Local:
                case ServiceTypes.ITunesPodcast:
                    // We already set the audio url
                    audioStream = AudioStreamUrl;
                    break;

                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    // SoundCloud has a fixed rate on playbacks. This system chooses a key on random and plays from it. These
                    // keys are provided by the web service (so more can be added when needed) so chances of expiring the key should
                    // be rare (especially when users start using YouTube and Fanburst Playback instead).

                    // Create list of keys with our default key
                    var apiKeys = new List<string>
                    {
                        service.ClientId
                    };

                    // Add backup keys
                    apiKeys.AddRange(service.ClientIds);

                    // Get random key
                    var randomNumber = new Random().Next(apiKeys.Count);
                    audioStream = $"https://api.soundcloud.com/tracks/{TrackId}/stream?client_id={apiKeys[randomNumber]}";
                    break;

                case ServiceTypes.YouTube:

                    if (IsLive)
                    {
                        audioStream = await youTubeClient.Videos.Streams.GetHttpLiveStreamUrlAsync(TrackId);
                    }
                    else
                    {
                        // Get the media streams for this YouTube video
                        var manifest = await youTubeClient.Videos.Streams.GetManifestAsync(TrackId);
                        audioStream = manifest.GetAudio().OrderByDescending(q => q.Bitrate).First()?.Url;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return audioStream;
        }

        /// <summary>
        ///     What service this track belongs to. Useful for
        ///     performing service specific tasks such as liking.
        /// </summary>
        [JsonProperty("service_type")]
        public int ServiceType { get; set; }

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Id of track
        /// </summary>
        [JsonProperty("track_id")]
        public string TrackId { get; set; }

        /// <summary>
        ///     Link to the track
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; } = "https://soundbytemedia.com/pages/open-default-link";

        /// <summary>
        ///     Is the track currently live (used for YouTube videos)
        /// </summary>
        [JsonProperty("is_live")]
        public bool IsLive { get; set; }

        /// <summary>
        ///     Url to the audio stream
        /// </summary>
        [JsonProperty("audio_stream_url")]
        public string AudioStreamUrl { get; set; }

        /// <summary>
        ///     Url to the video stream
        /// </summary>
        [JsonProperty("video_stream_url")]
        public string VideoStreamUrl { get; set; }

        /// <summary>
        ///     Artwork link
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        /// <summary>
        ///     Artwork link for thumbnails (smaller)
        /// </summary>
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        ///     Title of the track
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Description of the track
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     How long is the track
        /// </summary>
        [JsonProperty("duration")]
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///     Date and time the track was created/uploaded
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        ///     Amount of likes
        /// </summary>
        [JsonProperty("like_count")]
        public long LikeCount
        {
            get => _likeCount;
            set
            {
                if (_likeCount == value)
                    return;

                _likeCount = value;
                UpdateProperty();
            }
        }

        private long _likeCount;

        /// <summary>
        ///     Amount of dislikes (if supported - YouTube)
        /// </summary>
        [JsonProperty("dislike_count")]
        public long DislikeCount
        {
            get => _dislikeCount;
            set
            {
                if (_dislikeCount == value)
                    return;

                _dislikeCount = value;
                UpdateProperty();
            }
        }

        private long _dislikeCount;

        /// <summary>
        ///     Amount of views
        /// </summary>
        [JsonProperty("view_count")]
        public long ViewCount
        {
            get => _viewCount;
            set
            {
                if (_viewCount == value)
                    return;

                _viewCount = value;
                UpdateProperty();
            }
        }

        private long _viewCount;

        /// <summary>
        ///     Amount of comments
        /// </summary>
        [JsonProperty("comment_count")]
        public long CommentCount
        {
            get => _commentCount;
            set
            {
                if (_commentCount == value)
                    return;

                _commentCount = value;
                UpdateProperty();
            }
        }

        private long _commentCount;

        /// <summary>
        ///     Track Genre
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        ///     Has this track been liked
        /// </summary>
        [JsonProperty("is_liked")]
        public bool IsLiked
        {
            get => _isLiked;
            set
            {
                if (_isLiked == value)
                    return;

                _isLiked = value;
                UpdateProperty();
            }
        }

        private bool _isLiked;

        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        /// <summary>
        ///     The Track User
        /// </summary>
        [JsonProperty("user")]
        public BaseUser User
        {
            get => _user;
            set
            {
                if (_user == value)
                    return;

                _user = value;
                UpdateProperty();
            }
        }

        private BaseUser _user;

        /// <summary>
        ///     Custom properties you can set
        /// </summary>
        [JsonProperty("custom_properties")]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();

        #region Methods

        public void ToggleLike() => ToggleLikeAsync();

        private async Task ToggleLikeAsync()
        {
            if (IsLiked)
            {
                await Unlike();
            }
            else
            {
                await Like();
            }
        }

        public async Task Like()
        {
            // We have already liked the track
            if (IsLiked)
                return;

            // Make UI appear instant
            IsLiked = true;

            var hasLiked = false;

            switch (ServiceType)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    hasLiked = await new SoundCloudTrack(TrackId).LikeAsync();
                    break;

                case ServiceTypes.YouTube:
                    hasLiked = await new YouTubeTrack(TrackId).LikeAsync();
                    break;

                case ServiceTypes.Local: // Don't support liking
                    return;

                case ServiceTypes.ITunesPodcast: // Use SoundByte like
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsLiked = hasLiked;
        }

        public async Task Unlike()
        {
            // We have already unliked the track
            if (!IsLiked)
                return;

            // Make UI appear instant
            IsLiked = false;

            bool hasUnliked;

            switch (ServiceType)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    hasUnliked = await new SoundCloudTrack(TrackId).UnlikeAsync();
                    break;

                case ServiceTypes.YouTube:
                    hasUnliked = await new YouTubeTrack(TrackId).UnlikeAsync();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            IsLiked = !hasUnliked;
        }

        public async Task UpdateLikeStatusAsync()
        {
            try
            {
                switch (ServiceType)
                {
                    case ServiceTypes.SoundCloud:
                    case ServiceTypes.SoundCloudV2:
                        if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                            IsLiked = (await SoundByteService.Current.ExistsAsync(ServiceTypes.SoundCloud, "/me/favorites/" + TrackId)).Response;
                        else
                            IsLiked = false;
                        break;

                    case ServiceTypes.YouTube:
                        if (SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube))
                            IsLiked = (await SoundByteService.Current.GetAsync<YouTubeLikeHolder>(ServiceTypes.YouTube, "/videos/getRating", new Dictionary<string, string>
                            {
                                { "id", TrackId }
                            })).Response.Items.FirstOrDefault().Rating == "like";
                        else
                            IsLiked = false;
                        break;

                    default:
                        IsLiked = false;
                        break;
                }
            }
            catch
            {
                IsLiked = false;
            }
        }

        #endregion Methods

        #region Static Methods

        public static async Task<BaseSoundByteItem> GetTrackAsync(int service, string trackId)
        {
            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    return new BaseSoundByteItem((await SoundByteService.Current.GetAsync<SoundCloudTrack>(ServiceTypes.SoundCloud, $"/tracks/{trackId}")).Response.ToBaseTrack());

                case ServiceTypes.YouTube:
                    return new BaseSoundByteItem((await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceTypes.YouTube, "videos", new Dictionary<string, string>
                    {
                        {"part", "snippet,contentDetails"},
                        { "id", trackId }
                    })).Response.Tracks.FirstOrDefault()?.ToBaseTrack());

                default:
                    return null;
            }
        }

        public static async Task<IEnumerable<BaseSoundByteItem>> GetTracksAsync(int service, string trackIds)
        {
            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    return (await SoundByteService.Current.GetAsync<List<SoundCloudTrack>>(ServiceTypes.SoundCloud, $"/tracks?ids={trackIds}")).Response.Select(x => new BaseSoundByteItem(x.ToBaseTrack()));

                case ServiceTypes.YouTube:
                    return (await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceTypes.YouTube, "videos", new Dictionary<string, string>
                    {
                        { "part", "snippet,contentDetails"},
                        { "id", trackIds }
                    })).Response.Tracks.Select(x => new BaseSoundByteItem(x.ToBaseTrack()));

                default:
                    return new List<BaseSoundByteItem>();
            }
        }

        #endregion Static Methods
    }
}