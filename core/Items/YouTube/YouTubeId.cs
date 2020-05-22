using Newtonsoft.Json;

namespace SoundByte.Core.Items.YouTube
{
    [JsonObject]
    public class YouTubeId
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("channelId")]
        public string ChannelId { get; set; }

        [JsonProperty("videoId")]
        public string VideoId { get; set; }

        [JsonProperty("playlistId")]
        public string PlaylistId { get; set; }
    }
}