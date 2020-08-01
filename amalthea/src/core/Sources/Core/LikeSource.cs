using Flurl;
using Flurl.Http;
using Polly;
using SoundByte.Core.Extensions;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Models.Paging;
using SoundByte.Core.Models.Sources;
using SoundByte.Core.Services.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core.Sources.Core
{
    public class LikeSource : ISource
    {
        private readonly IAuthenticationService _authenticationService;

        public LikeSource(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

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

            if (await _authenticationService.IsSoundByteAccountConnectedAsync() == false)
                return new SourceResponse("Not signed in", "Sign in to or create a SoundByte Account to keep track of your liked songs across services.");

            return await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100))
                .ExecuteAsync(async () =>
                {
                    // Perform the request
                    var request = await Constants.SoundByteMediaWebsite
                        .AppendPathSegment("/api/v1/likes")
                        .SetQueryParams(new
                        {
                            client_id = Constants.SoundByteOAuthClientId,
                            offset = string.IsNullOrEmpty(token) ? "1" : token,
                            count = count
                        })
                        .WithHeader("User-Agent", "SoundByte App")
                        .GetSoundByteJsonAsync<LikesOutputModel>(_authenticationService);

                    // The user does not have any most played tracks
                    if (!request.Items.Any())
                    {
                        return new SourceResponse("No Items", "Like some music from other services to get started.");
                    }

                    var nextPage = request.Links.FirstOrDefault(x => x.Rel == "next_page");
                    if (nextPage == null)
                        return new SourceResponse(request.Items.ToArray(), "eol");

                    var param = new QueryParameterCollection(nextPage.Href);
                    var nextToken = param.FirstOrDefault(x => x.Key == "PageNumber").Value;

                    return new SourceResponse(request.Items.ToArray(), nextToken);
                });
        }

        public class LikesOutputModel
        {
            public PagingHeader Paging { get; set; }
            public List<LinkInfo> Links { get; set; }
            public List<Track> Items { get; set; }
        }
    }
}