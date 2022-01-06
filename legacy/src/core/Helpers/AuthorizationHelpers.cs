using SoundByte.Core.Exceptions;
using SoundByte.Core.Items;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SoundByte.Core.Helpers
{
    /// <summary>
    ///     These helpers are used for communicating with the SoundByte website.
    /// </summary>
    public static class AuthorizationHelpers
    {
        /// <summary>
        ///     Provide an auth code and a service name. This method calls the SoundByte
        ///     Website and performs login logic to get the auth token used in app.
        /// </summary>
        /// <param name="service">The service that this code belongs to.</param>
        /// <param name="authCode">The code you got from the login call</param>
        /// <returns></returns>
        public static async Task<LoginToken> GetAuthTokenAsync(int service, string authCode)
        {
            var serviceName = "";

            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    serviceName = "soundcloud";
                    break;

                case ServiceTypes.YouTube:
                    serviceName = "youtube";
                    break;
            }

            try
            {
                var result = await HttpService.Instance.PostAsync<SoundByteAuthHolder>($"https://dominicmaas.co.nz/api/soundbyte/auth/{serviceName}",
                    new Dictionary<string, string>
                    {
                        { "code", authCode }
                    });

                if (!result.Response.IsSuccess)
                {
                    throw new SoundByteException("Error Logging In", result.Response.ErrorMessage);
                }

                return result.Response.Token;
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("SoundByte Server Error", "There is currently an error with the SoundByte services. Please try again later. Message: " + hex.Message);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Error Logging In", ex.Message);
            }
        }

        public static async Task<LoginToken> GetNewAuthTokenAsync(int service, string refreshToken)
        {
            var serviceName = "";

            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    serviceName = "soundcloud";
                    break;

                case ServiceTypes.YouTube:
                    serviceName = "youtube";
                    break;
            }

            try
            {
                var result = await HttpService.Instance.PostAsync<SoundByteAuthHolder>($"https://dominicmaas.co.nz/api/soundbyte/refresh-auth/{serviceName}",
                    new Dictionary<string, string>
                    {
                        { "refresh_token", refreshToken }
                    });

                if (!result.Response.IsSuccess)
                {
                    throw new SoundByteException("Error Refreshing Token", result.Response.ErrorMessage);
                }

                return result.Response.Token;
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("SoundByte Server Error", "There is currently an error with the SoundByte services. Please try again later. Message: " + hex.Message);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Error Logging In", ex.Message);
            }
        }
    }
}