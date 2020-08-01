using Newtonsoft.Json;

namespace SoundByte.Core.Models.Media
{
    /// <summary>
    ///     Represents a podcast show in the application
    /// </summary>
    public class PodcastShow : Media
    {
        public PodcastShow() : base(MediaType.PodcastShow)
        { }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }
    }
}
