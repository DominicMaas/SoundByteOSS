using SoundByte.Core.Sources.SoundCloud;
using SoundByte.Core.Sources.YouTube;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.Generic
{
    public class RelatedTrackSource : ISource
    {
        public int Service { get; set; }
        public string TrackId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "i", TrackId },
                { "s", Service }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("i", out var trackId);
            data.TryGetValue("s", out var service);

            TrackId = trackId?.ToString();
            Service = int.Parse(service.ToString());
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (Service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    var scSource = Activator.CreateInstance<SoundCloudRelatedTrackSource>();
                    scSource.TrackId = TrackId;
                    return await scSource.GetItemsAsync(count, token, cancellationToken);

                case ServiceTypes.YouTube:
                    var ytSource = Activator.CreateInstance<YouTubeRelatedTrackSource>();
                    ytSource.TrackId = TrackId;
                    return await ytSource.GetItemsAsync(count, token, cancellationToken);

                default:
                    return new SourceResponse(null, null, false,
                        Resources.Resources.Sources_All_NotImplemented_Title, string.Format(Resources.Resources.Sources_All_NotImplemented_Description, Service));
            }
        }
    }
}