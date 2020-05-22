using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.YouTube.Playlist
{
    
    public class YouTubePlaylistItemSource : ISource
    {
        public string PlaylistId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "p", PlaylistId }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("p", out var playlistId);
            PlaylistId = playlistId.ToString();
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            // Call the YouTube API and get the videos
            var videos = await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceTypes.YouTube, "playlistItems",
                new Dictionary<string, string>
                {
                    {"part", "id,snippet,contentDetails"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"playlistId", PlaylistId}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no videos
            if (!videos.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No Items", "No Items");
            }

            // Convert YouTube specific videos to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            foreach (var track in videos.Response.Tracks)
            {
                if (track.Id.Kind == "youtube#video")
                {
                    baseTracks.Add(new BaseSoundByteItem(track.ToBaseTrack()));
                }
            }

            // Return the items
            return new SourceResponse(baseTracks, videos.Response.NextList);
        }
    }
}