using Newtonsoft.Json;
using SoundByte.Core.Items.Playlist;
using System.Collections.Generic;

namespace SoundByte.Core.Items.YouTube
{
    [JsonObject]
    public class YouTubePlaylistHolder
    {
        /// <summary>
        ///     Collection of playlists
        /// </summary>
        [JsonProperty("items")]
        public List<YouTubePlaylist> Playlists { get; set; }

        /// <summary>
        ///     The next list of items
        /// </summary>
        [JsonProperty("nextPageToken")]
        public string NextList { get; set; }
    }
}