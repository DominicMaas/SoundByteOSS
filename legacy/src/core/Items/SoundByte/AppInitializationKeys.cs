using Newtonsoft.Json;
using System.Collections.Generic;

namespace SoundByte.Core.Items.SoundByte
{
    /// <summary>
    ///     The keys that the SoundByte service returns.
    /// </summary>
    [JsonObject]
    public class AppInitializationKeys
    {
        public string SoundCloudClientId { get; set; }
        public List<string> SoundCloudPlaybackIds { get; set; }
        public string FanburstClientId { get; set; }
        public string YouTubeClientId { get; set; }
        public string YouTubeLoginClientId { get; set; }
        public string LastFmClientId { get; set; }
        public string GoogleAnalyticsTrackerId { get; set; }
        public string AppCenterClientId { get; set; }
        public string SoundByteClientId { get; set; }
        public string SpotifyClientId { get; set; }
        public string HockeyAppClientId { get; set; }
    }
}