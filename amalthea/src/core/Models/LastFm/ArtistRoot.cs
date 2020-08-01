using Newtonsoft.Json;
using System.Collections.Generic;

namespace SoundByte.Core.Models.LastFm
{
    public class ArtistRoot
    {
        [JsonProperty("artist")]
        public Artist? Artist { get; set; }
    }

    public class Artist
    {
        [JsonProperty("stats")]
        public Stats? Stats { get; set; }

        [JsonProperty("tags")]
        public Tags? Tags { get; set; }

        [JsonProperty("bio")]
        public Bio? Bio { get; set; }
    }

    public class Bio
    {
        [JsonProperty("summary")]
        public string? summary { get; set; }
    }

    public class Stats
    {
        [JsonProperty("listeners")]
        public string? Listeners { get; set; }

        [JsonProperty("playcount")]
        public string? PlayCount { get; set; }
    }

    public class Tag
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Tags
    {
        [JsonProperty("tag")]
        public List<Tag> Tag { get; set; }
    }
}
