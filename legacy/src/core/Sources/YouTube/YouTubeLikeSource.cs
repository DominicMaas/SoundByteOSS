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
    
    public class YouTubeLikeSource : ISource
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
                return await Task.Run(() =>
                    new SourceResponse(null, null, false,
                        Resources.Resources.Sources_YouTube_NoAccount_Title,
                        Resources.Resources.Sources_YouTube_Like_NoAccount_Description));
            }

            // Call the YouTube API and get the likes
            var likes = await SoundByteService.Current.GetAsync<YouTubeVideoHolder>(ServiceTypes.YouTube, "videos",
                new Dictionary<string, string>
                {
                    {"part", "id,snippet,contentDetails"},
                    {"maxResults", count.ToString()},
                    {"pageToken", token},
                    {"myRating", "like"}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!likes.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, Resources.Resources.Sources_All_NoResults_Title, Resources.Resources.Sources_YouTube_Like_NoItems_Description);
            }

            // Convert YouTube specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            foreach (var track in likes.Response.Tracks)
            {
                if (track.Id.Kind == "youtube#video")
                {
                    baseTracks.Add(new BaseSoundByteItem(track.ToBaseTrack()));
                }
            }

            // Return the items
            return new SourceResponse(baseTracks, likes.Response.NextList);
        }
    }
}