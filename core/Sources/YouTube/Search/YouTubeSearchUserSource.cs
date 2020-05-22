using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.YouTube.Search
{
    
    public class YouTubeSearchUserSource : ISource
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
            // Call the YouTube API and get the items
            var users = await SoundByteService.Current.GetAsync<YouTubeChannelHolder>(ServiceTypes.YouTube,
                "search",
                new Dictionary<string, string>
                {
                    {"part", "snippet"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"type", "channel"},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no users
            if (!users.Response.Channels.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title, string.Format(Resources.Resources.Sources_All_Search_NotResults_Description, SearchQuery));
            }

            // Convert YouTube specific channels to base users
            var baseUsers = new List<BaseSoundByteItem>();
            foreach (var user in users.Response.Channels)
            {
                if (user.Id.Kind == "youtube#channel")
                {
                    baseUsers.Add(new BaseSoundByteItem(user.ToBaseUser()));
                }
            }

            // Return the items
            return new SourceResponse(baseUsers, users.Response.NextList);
        }
    }
}