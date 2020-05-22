using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.YouTube
{
    
    public class YouTubePlaylistSource : ISource
    {
        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>();
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            // Not used
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // If the user has not connected their YouTube account.
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.YouTube))
            {
                return new SourceResponse(null, null, false,
                    Resources.Resources.Sources_YouTube_NoAccount_Title,
                    Resources.Resources.Sources_YouTube_Playlist_NoAccount_Description);
            }

            // Call the YouTube API and get the items
            var playlists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceTypes.YouTube, "playlists",
                new Dictionary<string, string>
                {
                    {"part", "id,snippet,contentDetails"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"mine", "true"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no playlists
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title,
                    Resources.Resources.Sources_YouTube_Playlist_NoItems_Description);
            }

            // Convert YouTube specific playlists to base playlists
            var basePlaylists = new List<BaseSoundByteItem>();
            foreach (var playlist in playlists.Response.Playlists)
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