using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Polly;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Paging;
using SoundByte.Core.Models.Sources;
using Xamarin.Essentials;

namespace SoundByte.Core.Sources.Podcasts
{
    public class PodcastSearchSource : ISource
    {
        private Dictionary<string, string> _parameters;

        public void ApplyParameters(Dictionary<string, string> data) => _parameters = data;

        public Dictionary<string, string> GetParameters() => _parameters;

        public async Task<SourceResponse> GetItemsAsync(int count, string token, CancellationToken cancellationToken = default)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return new SourceResponse("No Connection", "You are disconnected from the internet");

            // Extract the query
            var query = _parameters["query"];
            var country = "us";

            return await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100))
                .ExecuteAsync(async () =>
                {
                    // Perform the request
                    var request = await Constants.SoundByteMediaWebsite
                        .AppendPathSegment("/api/v1/podcasts/search")
                        .SetQueryParams(new
                        {
                            client_id = Constants.SoundByteOAuthClientId,
                            country,
                            query
                        })
                        .WithHeader("User-Agent", "SoundByte App")
                        .GetJsonAsync<List<PodcastShow>>(cancellationToken: cancellationToken);

                    // If there are no search results
                    if (!request.Any())
                    {
                        return new SourceResponse("No results found", "Could not find any podcasts matching " + query);
                    }

                    return new SourceResponse(request.ToArray(), "eol");
                });
        }
    }
}
