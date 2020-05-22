using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud
{
    public class SoundCloudStreamSource : ISource
    {
        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>();
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
            {
                // Call the SoundCloud API and get the items
                var items = await SoundByteService.Current.GetAsync<StreamTrackHolder>(ServiceTypes.SoundCloudV2, "/stream",
                    new Dictionary<string, string>
                    {
                        {"limit", count.ToString()},
                        {"linked_partitioning", "1"},
                        {"cursor", token}
                    }, cancellationToken).ConfigureAwait(false);

                // If there are no tracks
                if (!items.Response.Items.Any())
                {
                    return new SourceResponse(null, null, false, "No items", "Follow someone to get started.");
                }

                // Parse uri for cursor
                var param = new QueryParameterCollection(items.Response.NextList);
                var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

                // Convert the items to base items
                var baseItems = new List<BaseSoundByteItem>();
                foreach (var item in items.Response.Items)
                {
                    var type = ItemType.Unknown;

                    // Set the type depending on the SoundCloud String
                    // and if the object exists.
                    switch (item.Type)
                    {
                        case "track-repost":
                        case "track":
                            if (item.Track != null)
                                type = ItemType.Track;
                            break;

                        case "playlist-repost":
                        case "playlist":
                            if (item.Playlist != null)
                                type = ItemType.Playlist;
                            break;
                    }

                    // Add item to the list
                    switch (type)
                    {
                        case ItemType.Playlist:
                            baseItems.Add(new BaseSoundByteItem(item.Playlist?.ToBasePlaylist()));
                            break;

                        case ItemType.Track:
                            baseItems.Add(new BaseSoundByteItem(item.Track?.ToBaseTrack()));
                            break;
                    }
                }

                // Return the items
                return new SourceResponse(baseItems, nextToken);
            }

            return new SourceResponse(null, null, false, "SoundCloud account not connected.", "To view your SoundCloud Stream, login under the 'accounts' button.");
        }

        [JsonObject]
        private class StreamTrackHolder
        {
            /// <summary>
            ///     List of stream items
            /// </summary>
            [JsonProperty("collection")]
            public List<StreamItem> Items { get; set; }

            /// <summary>
            ///     Next items in the list
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }

        /// <summary>
        ///     A stream collection containing all items that may be on the users stream
        /// </summary>
        [JsonObject]
        private class StreamItem
        {
            /// <summary>
            ///     Track detail
            /// </summary>
            [JsonProperty("track")]
            public SoundCloudTrack Track { get; set; }

            /// <summary>
            ///     User detail
            /// </summary>
            [JsonProperty("user")]
            public SoundCloudUser User { get; set; }

            /// <summary>
            ///     Playlist detail
            /// </summary>
            [JsonProperty("playlist")]
            public SoundCloudPlaylist Playlist { get; set; }

            /// <summary>
            ///     When this object was created
            /// </summary>
            [JsonProperty("created_at")]
            public string CreatedAt { get; set; }

            /// <summary>
            ///     What type of object this is
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }
        }
    }
}