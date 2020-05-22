using System.Net.Http.Headers;

namespace SoundByte.Core.Items
{
    /// <summary>
    ///     Allows the ability to return content and response headers
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpReponse<T>
    {
        public HttpReponse()
        { }

        public HttpReponse(T response)
        {
            Response = response;
        }

        public HttpReponse(T response, HttpResponseHeaders headers)
        {
            Response = response;
            Headers = headers;
        }

        public HttpResponseHeaders Headers { get; set; }

        public T Response { get; set; }
    }
}