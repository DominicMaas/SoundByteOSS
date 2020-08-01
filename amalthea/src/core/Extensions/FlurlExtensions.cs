using Flurl.Http;
using SoundByte.Core.Models.MusicProvider;
using SoundByte.Core.Services.Definitions;
using System.Threading.Tasks;

namespace SoundByte.Core.Extensions
{
    public static class FlurlExtensions
    {
        public static async Task<T> GetSoundByteJsonAsync<T>(this IFlurlRequest req, IAuthenticationService authenticationService)
        {
            // Internal request
            var r = req;

            // If the users SoundByte account is connected, perform an OAuth request
            var accountToken = await authenticationService.GetSoundByteAccountTokenAsync();
            if (accountToken != null)
            {
                r = r.WithOAuthBearerToken(accountToken.AccessToken);
            }

            try
            {
                // Attempt to get the response
                return await r.GetJsonAsync<T>();
            }
            catch (FlurlHttpException ex)
            {
                // If an account token was provided, and
                if (accountToken != null && ex.Call.HttpStatus == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Refresh the token and store it
                    var newToken = await authenticationService.RefreshTokenAsync(accountToken.RefreshToken, null);
                    await authenticationService.ConnectSoundByteAccountAsync(newToken);

                    // Perform the request again
                    return await GetSoundByteJsonAsync<T>(req, authenticationService);
                }

                throw ex;
            }
        }

        public static async Task<string> HandleAuthExpireAsync(this IFlurlRequest req, IAuthenticationService authenticationService, MusicProvider musicProvider)
        {
            // Internal request
            var r = req;

            // If the music provider account is connected, perform an OAuth request
            var accountToken = await authenticationService.GetTokenAsync(musicProvider.Identifier);
            if (accountToken != null && musicProvider.Manifest.Authentication != null)
            {
                r = r.WithHeader("Authorization", $"{musicProvider.Manifest.Authentication.Scheme ?? "Bearer"} {accountToken.AccessToken}");
            }

            try
            {
                // Attempt to get the response
                return await r.GetStringAsync();
            }
            catch (FlurlHttpException ex)
            {
                // If an account token was provided, and
                if (accountToken != null && ex.Call.HttpStatus == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Refresh the token and store it
                    var newToken = await authenticationService.RefreshTokenAsync(accountToken.RefreshToken, musicProvider.Identifier);
                    await authenticationService.ConnectAccountAsync(musicProvider.Identifier, newToken);

                    // Perform the request again
                    return await HandleAuthExpireAsync(req, authenticationService, musicProvider);
                }

                throw ex;
            }
        }
    }
}