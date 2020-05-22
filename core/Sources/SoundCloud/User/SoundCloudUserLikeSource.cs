using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.SoundCloud.User
{
    
    public class SoundCloudUserLikeSource : ISource
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
            // Call the SoundCloud API and get the items
            var tracks = await SoundByteService.Current.GetAsync<LikeTrackHolder>(ServiceTypes.SoundCloud, $"/users/{UserId}/favorites",
                new Dictionary<string, string>
                {
                    {"limit", count.ToString()},
                    {"linked_partitioning", "1"},
                    {"cursor", token}
                }, cancellationToken).ConfigureAwait(false);

            // If there are no tracks
            if (!tracks.Response.Tracks.Any())
            {
                return new SourceResponse(null, null, false, "No likes", "This user has not liked any tracks");
            }

            // Parse uri for cursor
            var param = new QueryParameterCollection(tracks.Response.NextList);
            var nextToken = param.FirstOrDefault(x => x.Key == "cursor").Value;

            // Convert SoundCloud specific tracks to base tracks
            var baseTracks = new List<BaseSoundByteItem>();
            tracks.Response.Tracks.ForEach(x => baseTracks.Add(new BaseSoundByteItem(x.ToBaseTrack())));

            // Return the items
            return new SourceResponse(baseTracks, nextToken);
        }

        [JsonObject]
        private class LikeTrackHolder
        {
            /// <summary>
            ///     Collection of tracks
            /// </summary>
            [JsonProperty("collection")]
            public List<SoundCloudTrack> Tracks { get; set; }

            /// <summary>
            ///     The next list of items
            /// </summary>
            [JsonProperty("next_href")]
            public string NextList { get; set; }
        }
    }
}