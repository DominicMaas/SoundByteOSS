using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.User
{
    
    public class SoundCloudUserPlaylistSource : ISource
    {
        public string UserId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "u", UserId }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("u", out var userId);
            UserId = userId.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BaseSoundByteItem>();

            var endpoint = $"/users/{UserId}/playlists";
            var serviceType = ServiceTypes.SoundCloud;

            // Call the SoundCloud api and get the items
            var playlists = await SoundByteService.Current.GetAsync<UserPlaylistHolder>(serviceType, endpoint,
                new Dictionary<string, string>
                {
                        {"limit", count.ToString()},
                        {"offset", token},
                        {"linked_partitioning", "1"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse(null, null, false, "Nothing to hear here", "This user has uploaded no playlists");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(playlists.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            playlists.Response.Playlists.ForEach(x => basePlaylists.Add(new BaseSoundByteItem(x.ToBasePlaylist())));

            // Return the items
            return new SourceResponse(basePlaylists, nextToken);
        }

        [JsonObject]
        private class UserPlaylistHolder
        {
            /// <summary>
            ///     List of playlists
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudPlaylist> Playlists { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }

        [JsonObject]
        private class LikePlaylistBootstrap
        {
            /// <summary>
            ///     A playlist
            /// </summary>
            [JsonProperty("playlist")]
            public SoundCloudPlaylist Playlist { get; set; }
        }
    }
}