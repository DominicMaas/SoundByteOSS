using Newtonsoft.Json;
using SoundByte.Core.Converters.YouTube;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;
using System;

namespace SoundByte.Core.Items.Playlist
{
    [JsonObject]
    public class YouTubePlaylist : IPlaylist
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(YouTubePlaylistIdConverter))]
        public YouTubeId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSnippet Snippet { get; set; }

        [JsonProperty("contentDetails")]
        public ContentDetails YouTubeContentDetails { get; set; }

        [JsonObject]
        public class YouTubeSnippet
        {
            [JsonProperty("publishedAt")]
            public string PublishedAt { get; set; }

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
        }

        [JsonObject]
        public class ContentDetails
        {
            [JsonProperty("itemCount")]
            public int ItemCount { get; set; }
        }

        public BasePlaylist AsBasePlaylist => ToBasePlaylist();

        public BasePlaylist ToBasePlaylist()
        {
            return new BasePlaylist
            {
                ServiceType = ServiceTypes.YouTube,
                PlaylistId = Id.PlaylistId,
                Duration = TimeSpan.FromMilliseconds(0),
                Title = Snippet.Title,
                Genre = "YouTube",
                Description = Snippet.Description,
                CreationDate = DateTime.Parse(Snippet.PublishedAt),
                ArtworkUrl = Snippet.Thumbnails.HighSize.Url,
                ThumbnailUrl = Snippet.Thumbnails.MediumSize.Url,
                User = new BaseUser
                {
                    UserId = Snippet.ChannelId,
                    Username = Snippet.ChannelTitle
                },
                LikesCount = 0,
                TrackCount = YouTubeContentDetails?.ItemCount ?? 0
            };
        }
    }
}