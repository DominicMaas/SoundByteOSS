using Newtonsoft.Json;

namespace SoundByte.Core.Items.YouTube
{
    [JsonObject]
    public class YouTubeThumbnailSize
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public int? Width { get; set; }

        [JsonProperty("height")]
        public int? Height { get; set; }
    }
}