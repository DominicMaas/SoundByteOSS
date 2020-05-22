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
    
    public class YouTubeSearchPlaylistSource : ISource
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
            var playlists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceTypes.YouTube,
                "search",
                new Dictionary<string, string>
                {
                    {"part", "id"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"type", "playlist"},
                    {"q", WebUtility.UrlEncode(SearchQuery)}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no playlists
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title, string.Format(Resources.Resources.Sources_All_Search_NotResults_Description, SearchQuery));
            }

            // We now need to get the content details (ugh)
            var youTubeIdList = string.Join(",", playlists.Response.Playlists.Select(m => m.Id.PlaylistId));

            var extendedPlaylists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceTypes.YouTube,
                "playlists",
                new Dictionary<string, string>
                {
                    {"part", "snippet,contentDetails"},
                    {"id", youTubeIdList}
                }, cancellationToken).ConfigureAwait(false);

            // Convert YouTube specific playlists to base playlists
            var basePlaylists = new List<BaseSoundByteItem>();
            foreach (var playlist in extendedPlaylists.Response.Playlists)
            {
                if (playlist.Id.Kind == "youtube#playlist")
                {
                    basePlaylists.Add(new BaseSoundByteItem(playlist.ToBasePlaylist()));
                }
            }

            // Return the items
            return new SourceResponse(basePlaylists, playlists.Response.NextList);
        }
    }
}