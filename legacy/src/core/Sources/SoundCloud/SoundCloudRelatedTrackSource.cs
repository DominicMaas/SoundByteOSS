using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud
{

    public class SoundCloudRelatedTrackSource : ISource
    {
        public string TrackId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "i", TrackId }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("i", out var trackId);
            TrackId = trackId?.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                return new SourceResponse(null, null, false, "Not logged in",
                    "A connected SoundCloud account is required to view this content.");

            // Call the SoundCloud API and get the items
            var items = await SoundByteService.Current.GetAsync<TrackHolder>(ServiceTypes.SoundCloud, $"/tracks/{TrackId}/related",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"cursor", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!items.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No items", "Follow someone to get started.");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(items.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

            // Convert the items to base items
            var baseItems = new List<BaseSoundByteItem>();
            foreach (var item in items.Response.Tracks)
            {
                baseItems.Add(new BaseSoundByteItem(item.ToBaseTrack()));
            }

            // Return the items
            return new SourceResponse(baseItems, nextToken);
        }

        [JsonObject]
        private class TrackHolder
        {
            /// <summary>
            ///     Collection of tracks
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudTrack> Tracks { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}