using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.YouTube
{
    
    public class ExploreYouTubeTrendingSource : ISource
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
            // Call the YouTube API and get the items
            var tracks = await SoundByteService.Current.GetAsync<YouTubeExploreHolder>(ServiceTypes.YouTube, "videos",
                new Dictionary<string, string>
                {
                    {"part", "snippet,contentDetails"},
                    {"chart", "mostPopular"},
                    {"maxResults", count.ToString()},
                    {"videoCategoryId", "10"},
                    {"pageToken", token},
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No results found", "There are no trending YouTube items.");
            }

            // Convert YouTube specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            foreach (var track in tracks.Response.Tracks)
            {
                if (track.Id.Kind == "youtube#video")
                {
                    baseTracks.Add(new BaseSoundByteItem(track.ToBaseTrack()));
                }
            }

            // Return the items
            return new SourceResponse(baseTracks, tracks.Response.NextList);
        }

        [JsonObject]
        private class YouTubeExploreHolder
        {
            [JsonProperty("nextPageToken")]
            public string NextList { get; set; }

            [JsonProperty("items")]
            public List<YouTubeTrack> Tracks { get; set; }
        }
    }
}