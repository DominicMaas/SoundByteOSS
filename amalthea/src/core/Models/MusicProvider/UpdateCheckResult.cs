using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace SoundByte.Core.Models.MusicProvider
{
    public class UpdateCheckResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}