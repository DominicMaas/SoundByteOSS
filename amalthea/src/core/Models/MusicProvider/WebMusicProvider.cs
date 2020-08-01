using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace SoundByte.Core.Models.MusicProvider
{
    public class WebMusicProvider
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("downloads")]
        public int Downloads { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}