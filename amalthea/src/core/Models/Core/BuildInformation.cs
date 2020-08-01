using Newtonsoft.Json;

namespace SoundByte.Core.Models.Core
{
    /// <summary>
    ///     Generated information about the
    ///     current application build
    /// </summary>
    public class BuildInformation
    {
        /// <summary>
        ///     What branch was this build generated from
        /// </summary>
        [JsonProperty("branch")]
        public string Branch { get; set; }

        /// <summary>
        ///     What commit is this build generated from
        /// </summary>
        [JsonProperty("commit")]
        public string Commmit { get; set; }

        /// <summary>
        ///     What platform is this build for?
        /// </summary>
        [JsonProperty("platform")]
        public string Platform { get; set; }

        /// <summary>
        ///     The date and time of the build (in UTC)
        /// </summary>
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}