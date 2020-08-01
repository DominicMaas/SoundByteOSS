using Newtonsoft.Json;
using System;

namespace SoundByte.Core.Models.MusicProvider
{
    public class InstallResult
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("owner")]
        public bool Owner { get; set; }
    }
}