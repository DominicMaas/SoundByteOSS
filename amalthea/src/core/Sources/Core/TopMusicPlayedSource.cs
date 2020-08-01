using Flurl;
using Flurl.Http;
using Polly;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core.Sources.Core
{
    /// <summary>
    ///     Top played music on SoundByte
    /// </summary>
    public class TopMusicPlayedSource : ISource
    {
        public Dictionary<string, string> GetParameters()
        {
            return new Dictionary<string, string>();
        }

        public void ApplyParameters(Dictionary<string, string> data)
        { }

        public async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return new SourceResponse("No Connection", "You are disconnected from the internet");

            return await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100))
                .ExecuteAsync(async () =>
                {
                    // Perform the request
                    var request = await Constants.SoundByteMediaWebsite
                        .AppendPathSegment("/api/v1/top/music")
                        .SetQueryParams(new
                        {
                            client_id = Constants.SoundByteOAuthClientId,
                        })
                        .WithHeader("User-Agent", "SoundByte App")
                        .GetJsonAsync<List<Track>>(cancellationToken: cancellationToken);

                    // The user does not have any most played tracks
                    if (!request.Any())
                    {
                        return new SourceResponse("No Items", "There is no top music on SoundByte currently");
                    }

                    return new SourceResponse(request.ToArray(), "eol");
                });
        }
    }
}