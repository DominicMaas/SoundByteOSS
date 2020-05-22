using Newtonsoft.Json;
using SoundByte.Core.Converters.YouTube;
using SoundByte.Core.Items.YouTube;

namespace SoundByte.Core.Items.User
{
    [JsonObject]
    public class YouTubeUser : IUser
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(YouTubeChannelIdConverter))]
        public YouTubeId Id { get; set; }

        [JsonProperty("snippet")]
        public YouTubeSnippet Snippet { get; set; }

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

        public BaseUser ToBaseUser()
        {
            return new BaseUser
            {
                ServiceType = ServiceTypes.YouTube,
                UserId = Id.ChannelId,
                Username = Snippet.Title,
                ArtworkUrl = Snippet.Thumbnails.HighSize.Url,
                ThumbnailUrl = Snippet.Thumbnails.MediumSize.Url,
                Description = Snippet.Description
            };
        }
    }
}