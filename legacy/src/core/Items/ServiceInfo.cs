using JetBrains.Annotations;
using SoundByte.Core.Items.User;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SoundByte.Core.Items
{
    /// <summary>
    ///     Used to store information about a service for the new Core system.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class ServiceInfo
    {
        /// <summary>
        ///     What service this info belongs to
        /// </summary>
        public int Service { get; set; }

        /// <summary>
        ///     Client ID used to access API resources
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        ///     A list of backup client Ids
        /// </summary>
        public IEnumerable<string> ClientIds { get; set; }

        /// <summary>
        ///     The logged in users token
        /// </summary>
        public LoginToken UserToken { get; set; }

        /// <summary>
        ///     The current logged in user. This can be null if no user is logged in
        ///     with this account
        /// </summary>
        public BaseUser CurrentUser { get; set; }

        /// <summary>
        ///     The API url for this service. Must be formatted as such: {0} = endpoint,
        ///     {1} = Client ID. For example, 'https://soundbytemedia.com/api/v1/{0}?client_id={1}'
        /// </summary>
        public string ApiUrl { get; set; }

        public string ClientIdName { get; set; } = "client_id";

        public bool IncludeClientIdInAuthRequests { get; set; } = true;

        /// <summary>
        ///      Authentication scheme to use for this service, usually set to
        ///     Bearer or OAuth.
        /// </summary>
        public string AuthenticationScheme { get; set; }
    }
}