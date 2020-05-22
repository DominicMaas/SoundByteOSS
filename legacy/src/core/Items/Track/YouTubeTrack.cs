using Newtonsoft.Json;
using SoundByte.Core.Converters.YouTube;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;

namespace SoundByte.Core.Items.Track
{
    [JsonObject]
    public class YouTubeTrack : ITrack
    {
        public YouTubeTrack()
        {
        }

        public YouTubeTrack(string id)
        {
            Id = new YouTubeId { VideoId = id };
        }

        [JsonObject]
        public class YouTubeSnippet
        {
            [JsonProperty("publishedAt")]
            public string PublishedAt { get; set; }

            [JsonProperty("playlistId")]
            public string PlaylistId { get; set; }

            [JsonProperty("channelId")]
            public string ChannelId { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("description")]
            public string Description { get; set; }

            [JsonProperty("thumbnails")]
            public YouTubeThumbnails Thumbnails { get; set; }

            [JsonProperty("channelTitle")]
            public string ChannelTitle { get; set; }

            [JsonProperty("liveBroadcastContent")]
            public string LiveBroadcastContent { get; set; }
        }

        [JsonObject]
        public class YouTubeContentDetails
        {
            [JsonProperty("duration")]
            public string Duration { get; set; }

            [JsonProperty("videoId")]
            public string VideoId { get; set; }

            [JsonProperty("endAt")]
            public int? EndAt { get; set; }
        }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(YouTubeTrackIdConverter))]
        public YouTubeId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSnippet Snippet { get; set; }

        [JsonProperty("contentDetails")]
        public YouTubeContentDetails ContentDetails { get; set; }

        public BaseTrack AsBaseTrack => ToBaseTrack();

        /// <summary>
        /// Convert this YouTube specific track to a universal track.
        /// </summary>
        /// <returns></returns>
        public BaseTrack ToBaseTrack()
        {
            try
            {
                var track = new BaseTrack
                {
                    ServiceType = ServiceTypes.YouTube,
                    TrackId = string.IsNullOrEmpty(ContentDetails.VideoId) ? Id.VideoId : ContentDetails.VideoId,
                    ArtworkUrl = Snippet.Thumbnails?.HighSize?.Url,
                    ThumbnailUrl = Snippet.Thumbnails?.MediumSize?.Url,
                    Title = Snippet.Title,
                    Description = Snippet.Description,
                    Created = DateTime.Parse(Snippet.PublishedAt),
                    Duration = GetDuration(),
                    Genre = "YouTube",
                    IsLive = IsLive(),
                    User = new BaseUser
                    {
                        UserId = Snippet.ChannelId,
                        ServiceType = ServiceTypes.YouTube,
                        Username = Snippet.ChannelTitle,
                        ArtworkUrl = "http://a1.sndcdn.com/images/default_avatar_large.png",
                        ThumbnailUrl = "http://a1.sndcdn.com/images/default_avatar_large.png"
                    }
                };

                track.Link = $"https://www.youtube.com/watch?v={track.TrackId}";

                track.CustomProperties.Add("YouTubePlaylistItemId", Snippet?.PlaylistId);

                return track;
            }
            catch (Exception e)
            {
                throw new Exception("Error Decoding YouTube track: " + e.Message, e);
            }
        }

        private bool IsLive()
        {
            if (string.IsNullOrEmpty(Snippet.LiveBroadcastContent))
                return false;

            return Snippet.LiveBroadcastContent != "none";
        }

        private TimeSpan GetDuration()
        {
            if (ContentDetails == null)
            {
                return TimeSpan.FromMilliseconds(0);
            }

            if (string.IsNullOrEmpty(ContentDetails.Duration))
            {
                return ContentDetails.EndAt.HasValue
                    ? TimeSpan.FromSeconds(ContentDetails.EndAt.Value)
                    : TimeSpan.Zero;
            }

            try
            {
                return XmlConvert.ToTimeSpan(ContentDetails.Duration);
            }
            catch
            {
                return TimeSpan.FromMilliseconds(0);
            }
        }

        public async Task<bool> LikeAsync()
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube))
                return false;

            try
            {
                await SoundByteService.Current.PostAsync<object>(ServiceTypes.YouTube, "/videos/rate", "",
                    new Dictionary<string, string>
                    {
                        { "id", Id.VideoId },
                        { "rating",  "like"}
                    });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UnlikeAsync()
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube))
                return false;

            try
            {
                await SoundByteService.Current.PostAsync<object>(ServiceTypes.YouTube, "/videos/rate", "",
                    new Dictionary<string, string>
                    {
                        { "id", Id.VideoId },
                        { "rating",  "none"}
                    });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}