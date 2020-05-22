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
    
    public class SoundCloudExploreSource : ISource
    {
        /// <summary>
        ///     The genre to search for.
        /// </summary>
        public string Genre { get; set; } = "all-music";

        /// <summary>
        ///     The kind of item to search for
        /// </summary>
        public string Kind { get; set; } = "top";

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "filter", Genre }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("filter", out var genre);
            Genre = genre.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call the SoundCloud API and get the items
            var tracks = await SoundByteService.Current.GetAsync<ExploreTrackHolder>(ServiceTypes.SoundCloudV2, "/charts",
                new Dictionary<string, string>
                {
                    {"genre", "soundcloud%3Agenres%3A" + Genre},
                    {"kind", Kind},
                    {"limit", count.ToString()},
                    {"offset", token},
                    {"linked_partitioning", "1"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No results found", "No items matching");
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(tracks.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            tracks.Response.Tracks.ForEach(x => baseTracks.Add(new BaseSoundByteItem(x.Track.ToBaseTrack())));

            // Return the items
            return new SourceResponse(baseTracks, nextToken);
        }

        [JsonObject]
        private class ExploreTrackHolder
        {
            /// <summary>
            ///     Collection of tracks
            /// </summary>
            [JsonProperty("collection")]
            public List<ExploreTrackCollection> Tracks { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }

        [JsonObject]
        private class ExploreTrackCollection
        {
            /// <summary>
            ///     The track object
            /// </summary>
            [JsonProperty("track")]
            public SoundCloudTrack Track { get; set; }
        }
    }
}