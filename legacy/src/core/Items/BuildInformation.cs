using Newtonsoft.Json;

namespace SoundByte.Core.Items
{
    /// <summary>
    ///     Info about the current build
    /// </summary>
    [JsonObject]
    public class BuildInformation
    {
        /// <summary>
        ///     The branch that this was compliled from
        /// </summary>
        [JsonProperty("build_branch")]
        public string BuildBranch { get; set; }

        /// <summary>
        ///     The time this was built
        /// </summary>
        [JsonProperty("build_time")]
        public string BuildTime { get; set; }
    }
}