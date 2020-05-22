using Newtonsoft.Json;
using System;

namespace SoundByte.Core.Items.SoundByte
{
    /// <summary>
    ///     Data returned from the SoundByte app initialization event
    /// </summary>
    [JsonObject]
    public class AppInitializationResult
    {
        /// <summary>
        ///     If the server approves this connection (99.99% always true).
        /// </summary>
        [JsonProperty("success")]
        public bool Successful { get; set; }

        /// <summary>
        ///     If the server rejected this connection,
        ///     the title for the error.
        /// </summary>
        [JsonProperty("error_title")]
        public string ErrorTitle { get; set; }

        /// <summary>
        ///     If the server rejected this connection,
        ///     the description for the error.
        /// </summary>
        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Unique ID for this app.
        /// </summary>
        [JsonProperty("app_id")]
        public Guid AppId { get; set; }

        /// <summary>
        ///     API keys used by SoundByte.
        /// </summary>
        [JsonProperty("app_keys")]
        public AppInitializationKeys AppKeys { get; set; }
    }
}