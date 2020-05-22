using SoundByte.Core.Sources.SoundCloud.User;
using SoundByte.Core.Sources.YouTube.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Sources.Generic
{
    public class UserPlaylistSource : ISource
    {
        public int Service { get; set; }
        public string UserId { get; set; }

        public override Dictionary<string, object> GetParameters()
        {
            return new Dictionary<string, object>
            {
                { "i", UserId },
                { "s", Service }
            };
        }

        public override void ApplyParameters(Dictionary<string, object> data)
        {
            data.TryGetValue("i", out var userId);
            data.TryGetValue("s", out var service);

            UserId = userId.ToString();
            Service = int.Parse(service.ToString());
        }

        public override async Task<SourceResponse> GetItemsAsync(int count, string token,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            switch (Service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    var scSource = Activator.CreateInstance<SoundCloudUserPlaylistSource>();
                    scSource.UserId = UserId;
                    return await scSource.GetItemsAsync(count, token, cancellationToken);

                case ServiceTypes.YouTube:
                    var ytSource = Activator.CreateInstance<YouTubeUserPlaylistSource>();
                    ytSource.UserId = UserId;
                    return await ytSource.GetItemsAsync(count, token, cancellationToken);

                default:
                    return new SourceResponse(null, null, false,
                        Resources.Resources.Sources_All_NotImplemented_Title, string.Format(Resources.Resources.Sources_All_NotImplemented_Description, Service));
            }
        }
    }
}