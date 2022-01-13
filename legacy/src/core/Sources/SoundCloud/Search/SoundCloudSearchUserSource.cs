using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.Search
{

    public class SoundCloudSearchUserSource : ISource
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
            var users = await SoundByteService.Current.GetAsync<SearchUserHolder>(ServiceTypes.SoundCloud, "/users",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"offset", token},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no users
            if (!users.Response.Users.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title, string.Format(Resources.Resources.Sources_All_Search_NotResults_Description, SearchQuery));
            }

            // Parse uri for offset
            var param = new QueryParameterCollection(users.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "offset").Value;

            // Convert SoundCloud specific users to base users
            var baseUsers = new List<BaseSoundByteItem>();
            users.Response.Users.ForEach(x => baseUsers.Add(new BaseSoundByteItem(x.ToBaseUser())));

            // Return the items
            return new SourceResponse(baseUsers, nextToken);
        }

        [JsonObject]
        private class SearchUserHolder
        {
            /// <summary>
            ///     List of users
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudUser> Users { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}