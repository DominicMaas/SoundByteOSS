using Newtonsoft.Json;
using System.Collections.Generic;

namespace SoundByte.Core.Items.YouTube
{
    public class YouTubeLikeHolder
    {
        [JsonProperty("items")]
        public List<YouTubeLikeItem> Items { get; set; }
    }

    public class YouTubeLikeItem
    {
        [JsonProperty("videoId")]
        public string Id { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }
    }
}