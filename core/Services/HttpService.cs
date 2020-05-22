using Newtonsoft.Json;
using SoundByte.Core.Exceptions;
using SoundByte.Core.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SoundByte.Core.Services
{
    /// <summary>
    ///     A generic HTTP service that uses <see cref="HttpClient"/> for handling requests.
    /// </summary>
    public partial class HttpService : IHttpService, IDisposable
    {
        #region Private Variables

        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Internal instance of <see cref="HttpClient"/>.
        private HttpClient _client;

        // Used to deserialize data.
        private JsonSerializer _jsonSerializer;

        #endregion Private Variables

        #region Getters

        /// <summary>
        ///     Get the HTTP Client used in the class
        /// </summary>
        public HttpClient Client => _client;

        /// <summary>
        ///     Get the JsonSerializer used in this class
        /// </summary>
        public JsonSerializer Serializer => _jsonSerializer;

        #endregion Getters

        #region Constructors

        /// <summary>
        ///     Creates an instance of <see cref="HttpService"/> with a custom <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="client"></param>
        public HttpService(HttpClient client)
        {
            _client = client ?? throw new ArgumentException(nameof(client));

            _jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public HttpService()
        {
            var httpClientHandler = new HttpClientHandler();

            // Handle decompression
            if (httpClientHandler.SupportsAutomaticDecompression)
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            // Get version of dll
            var version = typeof(HttpService).GetTypeInfo().Assembly.GetName().Version;

            // Create the http client
            _client = new HttpClient(httpClientHandler, true);
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("SoundByte", $"{version.Major}.{version.Minor}.{version.Revision}"));

            _jsonSerializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        #endregion Constructors

        #region GET

        /// <summary>
        ///     Performs a GET request at the specified url. This method returns the response as
        ///     an object T.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to get</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        public async Task<HttpReponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Escape the url
            var escapedUri = new Uri(Uri.EscapeUriString(url));

            try
            {
                return await Task.Run(async () =>
                {
                    // Get the URL
                    using (var webRequest = await _client.GetAsync(escapedUri, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
                    {
                        // Ensure this request was successful
                        webRequest.EnsureSuccessStatusCode();

                        // Get the stream
                        using (var stream = await webRequest.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            // Read the stream
                            using (var streamReader = new StreamReader(stream))
                            {
                                // Get the text from the stream
                                using (var textReader = new JsonTextReader(streamReader))
                                {
                                    // Return the data
                                    return new HttpReponse<T>
                                    {
                                        Response = _jsonSerializer.Deserialize<T>(textReader),
                                        Headers = webRequest.Headers
                                    };
                                }
                            }
                        }
                    }
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return new HttpReponse<T>();
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException(Resources.Resources.HttpService_JsonError_Title,
                    Resources.Resources.HttpService_JsonError_Description + $"\n{jsex.Message}");
            }
        }

        #endregion GET

        #region POST

        /// <summary>
        ///     Performs a POST request at a specified url. This version takes a string-string
        ///     dictionary as the body.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to post</param>
        /// <param name="bodyParamaters">string-string dictionary as the body</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        public async Task<HttpReponse<T>> PostAsync<T>(string url, Dictionary<string, string> bodyParamaters, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Encode this content so we can send it.
            var encodedContent = new FormUrlEncodedContent(bodyParamaters);

            // Escape the url
            var escapedUri = new Uri(Uri.EscapeUriString(url));

            try
            {
                return await Task.Run(async () =>
                {
                    // Post this request to the url
                    using (var postQuery = await _client.PostAsync(escapedUri, encodedContent, cancellationToken).ConfigureAwait(false))
                    {
                        // Ensure this request was successful
                        postQuery.EnsureSuccessStatusCode();

                        // Get the stream
                        using (var stream = await postQuery.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        using (var streamReader = new StreamReader(stream))
                        using (var textReader = new JsonTextReader(streamReader))
                        {
                            return new HttpReponse<T>
                            {
                                Response = _jsonSerializer.Deserialize<T>(textReader),
                                Headers = postQuery.Headers
                            };
                        }
                    }
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return new HttpReponse<T>();
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException(Resources.Resources.HttpService_JsonError_Title,
                    Resources.Resources.HttpService_JsonError_Description + $"\n{jsex.Message}");
            }
        }

        /// <summary>
        ///     Performs a POST request at a specified url. This version takes a JSON string
        ///     as the body.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to post</param>
        /// <param name="body">json string as the body</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        public async Task<HttpReponse<T>> PostAsync<T>(string url, string body, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Encode this content so we can send it.
            var encodedContent = new StringContent(body, Encoding.UTF8, "application/json");

            // Escape the url
            var escapedUri = new Uri(Uri.EscapeUriString(url));

            try
            {
                return await Task.Run(async () =>
                {
                    // Post this request to the url
                    using (var postQuery = await _client.PostAsync(escapedUri, encodedContent, cancellationToken).ConfigureAwait(false))
                    {
                        // Ensure this request was successful
                        postQuery.EnsureSuccessStatusCode();

                        // Get the stream
                        using (var stream = await postQuery.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            // Read the stream
                            using (var streamReader = new StreamReader(stream))
                            {
                                // Get the text from the stream
                                using (var textReader = new JsonTextReader(streamReader))
                                {
                                    return new HttpReponse<T>
                                    {
                                        Response = _jsonSerializer.Deserialize<T>(textReader),
                                        Headers = postQuery.Headers
                                    };
                                }
                            }
                        }
                    }
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return new HttpReponse<T>();
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException(Resources.Resources.HttpService_JsonError_Title,
                    Resources.Resources.HttpService_JsonError_Description + $"\n{jsex.Message}");
            }
        }

        #endregion POST

        #region PUT

        /// <summary>
        ///     Performs a PUT request at a specified url. This version takes a string-string
        ///     dictionary as the body.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to put</param>
        /// <param name="bodyParamaters">string-string dictionary as the body</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns>An object of T</returns>
        public Task<HttpReponse<T>> PutAsync<T>(string url, Dictionary<string, string> bodyParamaters, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Performs a PUT request at a specified url. This version takes a JSON string
        ///     as the body.
        /// </summary>
        /// <typeparam name="T">Object type to deserialize into</typeparam>
        /// <param name="url">The url resource to put</param>
        /// <param name="body">json string as the body</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns></returns>
        public Task<HttpReponse<T>> PutAsync<T>(string url, string body, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<HttpReponse<bool>> PutAsync(string url, string body, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Encode this content so we can send it.
            var encodedContent = new StringContent(body, Encoding.UTF8, "application/json");

            // Escape the url
            var escapedUri = new Uri(Uri.EscapeUriString(url));

            try
            {
                return await Task.Run(async () =>
                {
                    // Post this request to the url
                    using (var putQuery = await _client.PutAsync(escapedUri, encodedContent, cancellationToken).ConfigureAwait(false))
                    {
                        return new HttpReponse<bool>
                        {
                            Response = putQuery.IsSuccessStatusCode,
                            Headers = putQuery.Headers
                        };
                    }
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return new HttpReponse<bool>
                {
                    Response = false
                };
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException(Resources.Resources.HttpService_JsonError_Title,
                    Resources.Resources.HttpService_JsonError_Description + $"\n{jsex.Message}");
            }
        }

        #endregion PUT

        #region DELETE

        /// <summary>
        ///     Deletes a specified resource. Will return true if the resource was deleted, else
        ///     false is the resource was not deleted.
        /// </summary>
        /// <param name="url">The url resource to delete</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns>True if the resource was deleted, false if not.</returns>
        public async Task<HttpReponse<bool>> DeleteAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Escape the url
            var escapedUri = new Uri(Uri.EscapeUriString(url));

            try
            {
                return await Task.Run(async () =>
                {
                    // Get the URL
                    using (var webRequest = await _client.DeleteAsync(escapedUri, cancellationToken).ConfigureAwait(false))
                    {
                        webRequest.EnsureSuccessStatusCode();

                        return new HttpReponse<bool>
                        {
                            Response = true,
                            Headers = webRequest.Headers
                        };
                    }
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return new HttpReponse<bool>
                {
                    Response = false
                };
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException(Resources.Resources.HttpService_JsonError_Title,
                    Resources.Resources.HttpService_JsonError_Description + $"\n{jsex.Message}");
            }
        }

        #endregion DELETE

        #region EXISTS

        /// <summary>
        ///     Checks to see if a resource is at the specified location. Returns true if the
        ///     resource exists, returns false if it does not. Will throw an exception incase of an
        ///     unknown error.
        /// </summary>
        /// <param name="url">The url to check</param>
        /// <param name="cancellationToken">Allows the ability to cancel the current task.</param>
        /// <returns>True if resource exists, false if not.</returns>
        public async Task<HttpReponse<bool>> ExistsAsync(string url, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Escape the url
            var escapedUri = new Uri(Uri.EscapeUriString(url));

            try
            {
                return await Task.Run(async () =>
                {
                    // Get the URL
                    using (var webRequest = await _client.GetAsync(escapedUri, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
                    {
                        // Return if the resource exists

                        return new HttpReponse<bool>
                        {
                            Response = webRequest.IsSuccessStatusCode,
                            Headers = webRequest.Headers
                        };
                    }
                }).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return new HttpReponse<bool>
                {
                    Response = false
                };
            }
            catch (JsonSerializationException jsex)
            {
                throw new SoundByteException(Resources.Resources.HttpService_JsonError_Title,
                    Resources.Resources.HttpService_JsonError_Description + $"\n{jsex.Message}");
            }
        }

        #endregion EXISTS

        #region Dispose Resources

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            _client.Dispose();

            _disposed = true;
        }

        #endregion Dispose Resources
    }

    /// <summary>
    /// A generic HTTP service that uses <see cref="HttpClient"/> for handling requests.
    /// </summary>
    public partial class HttpService
    {
        private static HttpService _instance;

        /// <summary>
        /// Singleton instance of <see cref="HttpService"/>.
        /// </summary>
        public static HttpService Instance => _instance ?? (_instance = new HttpService());
    }
}