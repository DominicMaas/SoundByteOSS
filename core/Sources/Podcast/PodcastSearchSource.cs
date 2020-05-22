/* |----------------------------------------------------------------|
 * | Copyright (c) 2017 - 2018 Grid Entertainment                   |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */

using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Podcast;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.Podcast
{
    
    public class PodcastSearchSource : ISource
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
            // Search for podcasts
            var podcasts = await SoundByteService.Current.GetAsync<Root>(ServiceTypes.ITunesPodcast, "/search", new Dictionary<string, string> {
                { "term", SearchQuery },
                { "country", "us" },
                { "entity", "podcast" }
            }, cancellationToken).ConfigureAwait(false);

            // If there are no podcasts
            if (!podcasts.Response.Items.Any())
            {
                return new SourceResponse(null, null, false, "No results found", "Could not find any results for '" + SearchQuery + "'");
            }

            // convert the items
            var baseItems = podcasts.Response.Items.Select(podcast => new BaseSoundByteItem(podcast.ToBasePodcast())).ToList();

            // return the items
            return new SourceResponse(baseItems, "eol");
        }

        [JsonObject]
        private class Root
        {
            [JsonProperty("results")]
            public List<ITunesPodcast> Items { get; set; }
        }
    }
}