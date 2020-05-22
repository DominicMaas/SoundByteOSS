using SoundByte.Core.Sources.SoundCloud.Playlist;
using SoundByte.Core.Sources.YouTube.Playlist;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.Generic
{
    public class GenericPlaylistItemSource : ISource
    {
        public int Service { get; set; }
        public string PlaylistId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "p", PlaylistId },
                { "s", Service }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("p", out var playlistId);
            data.TryGetValue("s", out var service);

            PlaylistId = playlistId.ToString();
            Service = int.Parse(service.ToString());
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (Service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    var scSource = Activator.CreateInstance<SoundCloudPlaylistItemSource>();
                    scSource.PlaylistId = PlaylistId;
                    return await scSource.GetItemsAsync(count, token, cancellationToken);

                case ServiceTypes.YouTube:
                    var ytSource = Activator.CreateInstance<YouTubePlaylistItemSource>();
                    ytSource.PlaylistId = PlaylistId;
                    return await ytSource.GetItemsAsync(count, token, cancellationToken);

                default:
                    return new SourceResponse(null, null, false,
                        Resources.Resources.Sources_All_NotImplemented_Title, string.Format(Resources.Resources.Sources_All_NotImplemented_Description, Service));
            }
        }
    }
}