using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.Search
{

    public class SoundCloudSearchPlaylistSource : ISource
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

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                return new SourceResponse(null, null, false, "Not logged in",
                    "A connected SoundCloud account is required to view this content.");

            // Call the SoundCloud API and get the items
            var playlists = await SoundByteService.Current.GetAsync<SearchPlaylistHolder>(ServiceTypes.SoundCloud, "/playlists",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no playlists
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title, string.Format(Resources.Resources.Sources_All_Search_NotResults_Description, SearchQuery));
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(playlists.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific playlists to base playlists
            var basePlaylists = new List<BaseSoundByteItem>();
            playlists.Response.Playlists.ForEach(x => basePlaylists.Add(new BaseSoundByteItem(x.ToBasePlaylist())));

            // Return the items
            return new SourceResponse(basePlaylists, nextToken);
        }

        [JsonObject]
        private class SearchPlaylistHolder
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
    }
}