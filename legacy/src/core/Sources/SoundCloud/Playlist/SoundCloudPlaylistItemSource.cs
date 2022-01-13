using JetBrains.Annotations;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Playlist;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.Playlist
{

    public class SoundCloudPlaylistItemSource : ISource
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
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                return new SourceResponse(null, null, false, "Not logged in",
                    "A connected SoundCloud account is required to view this content.");

            // Call the SoundCloud API and get the playlist items
            var tracks = await SoundByteService.Current.GetAsync<SoundCloudPlaylist>(ServiceTypes.SoundCloud, "playlists/" + PlaylistId, null, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No Items", "No Items");
            }

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            tracks.Response.Tracks.ForEach(x => baseTracks.Add(new BaseSoundByteItem(x.ToBaseTrack())));

            // Return the items
            return new SourceResponse(baseTracks, "eol");
        }
    }
}