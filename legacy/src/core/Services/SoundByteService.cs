using JetBrains.Annotations;
using Newtonsoft.Json;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Helpers;
using SoundByte.Core.Items;
using SoundByte.Core.Items.User;
using SoundByte.Core.Items.YouTube;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Services
{
    /// <summary>
    ///     Next generation (Gen3.0) SoundByte Service. New features include portability (.NET Standard),
    ///     events (event based e.g OnServiceConnected), muiltiple services, easy to extend.
    /// </summary>
    public class SoundByteService
    {
        #region Delegates

        public delegate void ServiceConnectedEventHandler(int type, LoginToken token);

        public delegate void ServiceDisconnectedEventHandler(int type, string reason);

        #endregion Delegates

        public bool IsPreview { get; set; }

        #region Events

        /// <summary>
        /// This event is fired when a service is connected to SoundByte.
        /// When this event fires you should store the token somewhere within
        /// your application.
        /// </summary>
        public event ServiceConnectedEventHandler OnServiceConnected;

        /// <summary>
        /// This event is fired when a service is disconnected. When this event fires
        /// you should remove any saved tokens and update the appropriate UI.
        /// </summary>
        public event ServiceDisconnectedEventHandler OnServiceDisconnected;

        #endregion Events

        #region Private Variables

        // Has this class performed basic load yet (using Init();)
        private bool _isLoaded;

        /// <summary>
        /// List of services and their client id / client secrets.
        /// Also contains login information.
        /// </summary>
        public List<ServiceInfo> Services { get; } = new List<ServiceInfo>();

        #endregion Private Variables

        #region Instance Setup

        private static readonly Lazy<SoundByteService> InstanceHolder =
            new Lazy<SoundByteService>(() => new SoundByteService());

        /// <summary>
        /// Gets the current instance of SoundByte V3 Service
        /// </summary>
        public static SoundByteService Current => InstanceHolder.Value;

        /// <summary>
        /// Setup the service
        /// </summary>
        /// <param name="services">A list of services that will be used in the app</param>
        public void Init(IEnumerable<ServiceInfo> services)
        {
            // Empty any other secrets
            Services.Clear();

            // Loop through all the keys and add them
            foreach (var service in services)
            {
                // If there is already a service in the list, thow an exception, there
                // should only be one key for each service.
                if (Services.FirstOrDefault(x => x.Service == service.Service) != null)
                    throw new Exception("Only one key for each service!");

                Services.Add(service);
            }

            _isLoaded = true;
        }

        /// <summary>
        ///     Init logged in user objects. This should be called in the background after the app
        ///     has started.
        /// </summary>
        /// <returns></returns>
        public async Task<string> InitUsersAsync()
        {
            var errorString = string.Empty;

            foreach (var service in Services)
            {
                // Don't run if the user has not logged in
                if (service.UserToken == null) continue;

                try
                {
                    switch (service.Service)
                    {
                        case ServiceTypes.SoundCloud:
                        case ServiceTypes.SoundCloudV2:
                            service.CurrentUser = (await GetAsync<SoundCloudUser>(ServiceTypes.SoundCloud, "/me").ConfigureAwait(false)).Response.ToBaseUser();
                            break;

                        case ServiceTypes.YouTube:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<YouTubeChannelHolder>(ServiceTypes.YouTube, "/channels", new Dictionary<string, string>
                            {
                                { "mine", "true" },
                                { "part", "snippet" }
                            }).ConfigureAwait(false)).Response.Channels.FirstOrDefault()?.ToBaseUser();
                            break;

                        case ServiceTypes.SoundByte:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<BaseUser>(ServiceTypes.SoundByte, "/me").ConfigureAwait(false)).Response;
                            break;
                    }
                }
                catch (SoundByteException ex)
                {
                    if (ex.Message.Contains("401"))
                    {
                        errorString +=
                            $"There is an issue with your {service.Service.ToString()} account. Account has been disconnected. Please login again under 'Accounts'.\n";
                        DisconnectService(service.Service, $"There is an issue with your {service.Service.ToString()} account. Account has been disconnected. Please login again under 'Accounts'.");
                    }
                }
            }

            return errorString;
        }

        #endregion Instance Setup

        #region Service Methods

        /// <summary>
        /// Returns the user object of the connected service. Please note,
        /// this value can be null if the user has not logged in.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BaseUser GetConnectedUser(int type)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Check that the service actually exists
            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceNotExistException(type);

            // If the user token is not null, but the user is null, update the user
            if (service.UserToken != null && service.CurrentUser == null)
            {
                try
                {
                    switch (service.Service)
                    {
                        case ServiceTypes.SoundCloud:
                        case ServiceTypes.SoundCloudV2:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<SoundCloudUser>(ServiceTypes.SoundCloud, "/me").ConfigureAwait(false)).Response.ToBaseUser();
                            break;

                        case ServiceTypes.YouTube:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<YouTubeChannelHolder>(ServiceTypes.YouTube, "/channels", new Dictionary<string, string>
                            {
                                { "mine", "true" },
                                { "part", "snippet" }
                            }).ConfigureAwait(false)).Response.Channels.FirstOrDefault()?.ToBaseUser();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Could not load user: " + ex.Message);
                }
            }

            // Return the connected user
            return service.CurrentUser;
        }

        /// <summary>
        /// Connects a service to SoundByte. This will allow accessing
        /// user content. The ServiceConnected event is fired.
        /// </summary>
        /// <param name="type">The service to connect.</param>
        /// <param name="token">The required token</param>
        public void ConnectService(int type, LoginToken token)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceNotExistException(type);

            // Set the token
            service.UserToken = token;

            if (service.UserToken != null)
            {
                try
                {
                    switch (service.Service)
                    {
                        case ServiceTypes.SoundCloud:
                        case ServiceTypes.SoundCloudV2:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<SoundCloudUser>(ServiceTypes.SoundCloud, "/me").ConfigureAwait(false)).Response.ToBaseUser();
                            break;

                        case ServiceTypes.SoundByte:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<BaseUser>(ServiceTypes.SoundByte, "/me").ConfigureAwait(false)).Response;
                            break;

                        case ServiceTypes.YouTube:
                            service.CurrentUser = AsyncHelper.RunSync(async () => await GetAsync<YouTubeChannelHolder>(ServiceTypes.YouTube, "/channels", new Dictionary<string, string>
                            {
                                { "mine", "true" },
                                { "part", "snippet" }
                            }).ConfigureAwait(false)).Response.Channels.FirstOrDefault()?.ToBaseUser();
                            break;
                    }
                }
                catch
                {
                    // Todo: There are many reasons why this could fail.
                    // For now we just delete the user token
                    service.UserToken = null;
                }
            }

            // Fire the event
            OnServiceConnected?.Invoke(type, token);
        }

        /// <summary>
        /// Disconnects a specified service from SoundByte and
        /// fires the service disconnected event handler.
        /// </summary>
        /// <param name="type">The service to disconnect</param>
        /// <param name="reason">Why the service was disconnected</param>
        public void DisconnectService(int type, string reason)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Get the service information
            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceNotExistException(type);

            // Delete the user token
            service.UserToken = null;
            service.CurrentUser = null;

            // Fire the event
            OnServiceDisconnected?.Invoke(type, reason);
        }

        /// <summary>
        ///     Has the user logged in with their SoundByte account.
        /// </summary>
        public bool IsSoundByteAccountConnected => IsServiceConnected(ServiceTypes.SoundByte);

        /// <summary>
        /// Is the user logged into a service. Warning: will throw an exception if
        /// the service does not exsit.
        /// </summary>
        /// <param name="type">The service to check if the user has connected.</param>
        /// <returns>If the user accounted is connected</returns>
        public bool IsServiceConnected(int type)
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Get the service information
            var service = Services.FirstOrDefault(x => x.Service == type);

            if (service == null)
                throw new ServiceNotExistException(type);

            // If the user token is not null, we are connected
            return service.UserToken != null;
        }

        #endregion Service Methods

        #region Web API

        /// <summary>
        ///     This method builds the request url for the specified service.
        /// </summary>
        /// <param name="type">The service type to build the request url</param>
        /// <param name="endpoint">User defiend endpoint</param>
        /// <param name="optionalParams"></param>
        /// <returns>Fully build request url</returns>
        private string BuildRequestUrl(int type, string endpoint, Dictionary<string, string> optionalParams)
        {
            // Strip out the / infront of the endpoint if it exists
            endpoint = endpoint.TrimStart('/');

            // Get the requested service
            var service = Services.FirstOrDefault(x => x.Service == type);
            if (service == null)
                throw new ServiceNotExistException(type);

            // Build the base url
            var requestUri = string.Format(service.ApiUrl, endpoint, service.ClientId);

            // Check that there are optional params then loop through all
            // the params and add them onto the request URL
            if (optionalParams != null)
                requestUri = optionalParams
                    .Where(param => !string.IsNullOrEmpty(param.Key) && !string.IsNullOrEmpty(param.Value))
                    .Aggregate(requestUri, (current, param) => current + "&" + param.Key + "=" + param.Value);

            return requestUri;
        }

        /// <summary>
        ///     Adds the required headers to the http service depending on
        ///     the service type. This defaults to OAuth, and uses Bearer for
        ///     YouTube and Fanburst
        /// </summary>
        /// <param name="service">Http Service to append the headers.</param>
        /// <param name="type">What type of service is this user accessing.</param>
        private void BuildAuthLayer(HttpService service, int type)
        {
            // Add the service only if it's connected
            if (IsServiceConnected(type))
            {
                // Get the token and scheme
                var scheme = Services.FirstOrDefault(x => x.Service == type)?.AuthenticationScheme;
                var token = Services.FirstOrDefault(x => x.Service == type)?.UserToken?.AccessToken;

                // Add the auth request
                service.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
            }
        }

        /// <summary>
        ///     Handles token refreshing when the access token expires
        /// </summary>
        /// <param name="hex">HttpRequestException exception</param>
        /// <param name="type">The service type</param>
        /// <returns>If we refreshed the refresh token</returns>
        private async Task<bool> HandleAuthTokenRefreshAsync(HttpRequestException hex, int type)
        {
            if (!hex.Message.ToLower().Contains("401") || !IsServiceConnected(type))
                return false;

            try
            {
                // Get the token
                var userToken = Services.FirstOrDefault(x => x.Service == type)?.UserToken;
                if (userToken != null)
                {
                    var newToken = await AuthorizationHelpers.GetNewAuthTokenAsync(type, userToken.RefreshToken);

                    if (!string.IsNullOrEmpty(newToken.AccessToken))
                        userToken.AccessToken = newToken.AccessToken;

                    if (!string.IsNullOrEmpty(newToken.RefreshToken))
                        userToken.RefreshToken = newToken.RefreshToken;

                    if (!string.IsNullOrEmpty(newToken.ExpireTime))
                        userToken.ExpireTime = newToken.ExpireTime;

                    // Reconnect the service
                    ConnectService(type, userToken);

                    return true;
                }
            }
            catch (SoundByteException)
            {
                DisconnectService(type, "There is an issue with your '" + type + "' account and SoundByte has disconnected it. To connect your account again, go to 'Accounts' and click login.");
            }

            return false;
        }

        /// <summary>
        ///     Fetches an object from the specified service API and returns it.
        /// </summary>
        public async Task<HttpReponse<T>> GetAsync<T>(int type, string endpoint, Dictionary<string, string> optionalParams = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.GetAsync<T>(requestUri, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await GetAsync<T>(type, endpoint, optionalParams, cancellationToken);

                throw new SoundByteException("Resource Unavailable", hex.Message, hex);
            }
        }

        /// <summary>
        ///     Fetches a generic url
        /// </summary>
        public async Task<HttpReponse<T>> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Perform HTTP request
                    return await httpService.GetAsync<T>(requestUri, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                throw new SoundByteException("Resource Unavailable", hex.Message, hex);
            }
        }

        /// <summary>
        ///     This method allows the ability to perform a PUT command at a certain API method. Also
        ///     adds required OAuth token.
        ///     Returns if the PUT request has successful or not
        /// </summary>
        public async Task<HttpReponse<bool>> PutAsync(int type, string endpoint, string content = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Start building the request URL
            var requestUri = BuildRequestUrl(type, endpoint, null);

            // We have to have content (ugh)
            if (string.IsNullOrEmpty(content))
                content = "n/a";

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PutAsync(requestUri, content, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PutAsync(type, endpoint, content, cancellationToken);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
            catch (Exception ex)
            {
                throw new SoundByteException("Something went wrong", ex.Message);
            }
        }

        /// <summary>
        ///     Contacts the specified API and posts the content.
        /// </summary>
        public async Task<HttpReponse<T>> PostAsync<T>(int type, string endpoint, string content = null, Dictionary<string, string> optionalParams = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            // We have to have content (ugh)
            if (string.IsNullOrEmpty(content))
                content = "n/a";

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PostAsync<T>(requestUri, content, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PostAsync<T>(type, endpoint, content, optionalParams, cancellationToken);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
        }

        public async Task<HttpReponse<T>> PostAsync<T>(int type, string endpoint, Dictionary<string, string> bodyParams = null,
            Dictionary<string, string> optionalParams = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            if (bodyParams == null)
                bodyParams = new Dictionary<string, string>();

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PostAsync<T>(requestUri, bodyParams, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PostAsync<T>(type, endpoint, bodyParams, optionalParams, cancellationToken);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
        }

        public async Task<HttpReponse<T>> PostItemAsync<T>(int type, string endpoint, T item, Dictionary<string, string> optionalParams = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.PostAsync<T>(requestUri, JsonConvert.SerializeObject(item), cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                if (await HandleAuthTokenRefreshAsync(hex, type))
                    return await PostItemAsync(type, endpoint, item, optionalParams, cancellationToken);

                throw new SoundByteException("No connection?", hex.Message + "\n" + requestUri);
            }
        }

        /// <summary>
        ///    Attempts to delete an object from the specified API
        /// </summary>
        public async Task<HttpReponse<bool>> DeleteAsync(int type, string endpoint, Dictionary<string, string> optionalParams = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, optionalParams);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.DeleteAsync(requestUri, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                try
                {
                    if (await HandleAuthTokenRefreshAsync(hex, type))
                        return await DeleteAsync(type, endpoint, optionalParams, cancellationToken);
                }
                catch
                {
                    return new HttpReponse<bool>(false);
                }

                return new HttpReponse<bool>(false);
            }
            catch
            {
                return new HttpReponse<bool>(false);
            }
        }

        /// <summary>
        ///     Checks to see if an items exists at the specified endpoint
        /// </summary>
        public async Task<HttpReponse<bool>> ExistsAsync(int type, string endpoint, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_isLoaded == false)
                throw new SoundByteNotLoadedException();

            // Build the request Url
            var requestUri = BuildRequestUrl(type, endpoint, null);

            try
            {
                using (var httpService = new HttpService())
                {
                    // Accept JSON
                    httpService.Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    // Build the required auth headers
                    BuildAuthLayer(httpService, type);

                    // Perform HTTP request
                    return await httpService.ExistsAsync(requestUri, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (HttpRequestException hex)
            {
                try
                {
                    if (await HandleAuthTokenRefreshAsync(hex, type))
                        return await ExistsAsync(type, endpoint, cancellationToken);
                }
                catch
                {
                    return new HttpReponse<bool>(false);
                }

                return new HttpReponse<bool>(false);
            }
            catch
            {
                return new HttpReponse<bool>(false);
            }
        }
    }

    #endregion Web API
}