using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.YouTube.User
{
    
    public class YouTubeUserPlaylistSource : ISource
    {
        public string UserId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "i", UserId }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("i", out var userId);
            UserId = userId.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call the YouTube API and get the items
            var playlists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceTypes.YouTube, "search",
                new Dictionary<string, string>
                {
                    {"part", "id"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"type", "playlist"},
                    {"channelId", UserId},
                    {"order","date"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no playlists
            if (!playlists.Response.Playlists.Any())
            {
                return new SourceResponse(null, null, false, "No playlists", "This user has not created any playlists.");
            }

            // We now need to get the content details (ugh)
            var youTubeIdList = string.Join(",", playlists.Response.Playlists.Select(m => m.Id.PlaylistId));

            var extendedPlaylists = await SoundByteService.Current.GetAsync<YouTubePlaylistHolder>(ServiceTypes.YouTube, "playlists",
                new Dictionary<string, string>
                {
                    {"part","snippet,contentDetails"},
                    {"id", youTubeIdList}
                }, cancellationToken).ConfigureAwait(false);

            // Convert YouTube specific playlists to base playlists
            var basePlaylists = new List<BaseSoundByteItem>();
            foreach (var track in extendedPlaylists.Response.Playlists)
            {
                if (track.Id.Kind == "youtube#playlist")
                {
                    basePlaylists.Add(new BaseSoundByteItem(track.ToBasePlaylist()));
                }
            }

            // Return the items
            return new SourceResponse(basePlaylists, playlists.Response.NextList);
        }
    }
}