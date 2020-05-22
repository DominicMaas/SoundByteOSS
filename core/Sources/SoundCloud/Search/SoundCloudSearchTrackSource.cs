using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.Search
{
    /// <summary>
    /// Searches the SoundCloud API for tracks
    /// </summary>
    
    public class SoundCloudSearchTrackSource : ISource
    {
        /// <summary>
        /// What we should search for
        /// </summary>
        public string SearchQuery { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "q", SearchQuery }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("q", out var query);
            SearchQuery = query.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call the SoundCloud API and get the items
            var tracks = await SoundByteService.Current.GetAsync<TrackListHolder>(ServiceTypes.SoundCloud, "/tracks",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title, string.Format(Resources.Resources.Sources_All_Search_NotResults_Description, SearchQuery));
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(tracks.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            tracks.Response.Tracks.ForEach(x => baseTracks.Add(new BaseSoundByteItem(x.ToBaseTrack())));

            // Return the items
            return new SourceResponse(baseTracks, nextToken);
        }

        /// <summary>
        /// Private class used to decode SoundCloud tracks
        /// </summary>
        [JsonObject]
        private class TrackListHolder
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