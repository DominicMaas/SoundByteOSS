using Newtonsoft.Json;

namespace SoundByte.Core.Items.YouTube
{
    [JsonObject]
    public class YouTubeThumbnails
    {
        [JsonProperty("default")]
        public YouTubeThumbnailSize DefaultSize { get; set; }

        [JsonProperty("medium")]
        public YouTubeThumbnailSize MediumSize { get; set; }

        [JsonProperty("high")]
        public YouTubeThumbnailSize HighSize { get; set; }
    }
}