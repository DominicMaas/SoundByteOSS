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

namespace SoundByte.Core.Sources.SoundCloud
{

    public class SoundCloudLikedPlaylistSource : ISource
    {
        public override Dictionary<string, object> GetParameters()
        {
            // Not used
            return new Dictionary<string, object>();
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                return new SourceResponse(null, null, false, "Not logged in",
                    "A connected SoundCloud account is required to view this content.");

            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BaseSoundByteItem>();

            var endpoint = $"/users/{SoundByteService.Current.GetConnectedUser(ServiceTypes.SoundCloud)?.UserId}/likes/playlists";
            var serviceType = ServiceTypes.SoundCloud;

            // Call the SoundCloud api and get the items
            var playlists = await SoundByteService.Current.GetAsync<UserLikePlaylistHolder>(serviceType, endpoint,
                new Dictionary<string, string>
                {
                        {"limit", count.ToString()},
                        {"offset", token},
                        {"linked_partitioning", "1"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!playlists.Response.Collection.Any())
            {
                return new SourceResponse(null, null, false, "Nothing to hear here",
                    "This user has uploaded no playlists");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(playlists.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            playlists.Response.Collection.ForEach(x => basePlaylists.Add(new BaseSoundByteItem(x.ToBasePlaylist())));

            // Return the items
            return new SourceResponse(basePlaylists, nextToken);
        }

        [JsonObject]
        private class UserLikePlaylistHolder
        {
            /// <summary>
            ///     List of playlists
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudPlaylist> Collection { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}