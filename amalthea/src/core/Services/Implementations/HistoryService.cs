using Flurl;
using Flurl.Http;
using Polly;
using SoundByte.Core.Models.Media;
using SoundByte.Core.Services.Definitions;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundByte.Core.Services.Implementations
{
    public class HistoryService : IHistoryService
    {
        private readonly IAuthenticationService _authenticationService;

        public HistoryService(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task AddToHistoryAsync(Media media)
        {
            // No network, don't worry (TODO, add to local cache)
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                return;

            if (await _authenticationService.IsSoundByteAccountConnectedAsync() && media.MediaType == MediaType.Track)
            {
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100))
                    .ExecuteAsync(async () =>
                    {
                        // If the users SoundByte account is connected, perform an OAuth request
                        var accountToken = await _authenticationService.GetSoundByteAccountTokenAsync();

                        try
                        {
                            // Perform the request
                            var request = await Constants.SoundByteMediaWebsite
                                .AppendPathSegment("/api/v1/history")
                                .SetQueryParams(new
                                {
                                    client_id = Constants.SoundByteOAuthClientId,
                                })
                                .WithHeader("User-Agent", "SoundByte App")
                                .WithOAuthBearerToken(accountToken.AccessToken)
                                .PostJsonAsync(media);
                        }
                        catch (FlurlHttpException ex)
                        {
                            // If an account token was provided, and
                            if (accountToken != null && ex.Call.HttpStatus == System.Net.HttpStatusCode.Unauthorized)
                            {
                                // Refresh the token and store it
                                var newToken = await _authenticationService.RefreshTokenAsync(accountToken.RefreshToken, null);
                                await _authenticationService.ConnectSoundByteAccountAsync(newToken);

                                // Perform the request again
                                await AddToHistoryAsync(media);
                                return;
                            }

                            throw ex;
                        }
                    });
            }
        }
    }
}
