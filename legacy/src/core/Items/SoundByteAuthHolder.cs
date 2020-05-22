using Newtonsoft.Json;

namespace SoundByte.Core.Items
{
    [JsonObject]
    public class SoundByteAuthHolder
    {
        [JsonProperty("successful")]
        public bool IsSuccess { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("login_token")]
        public LoginToken Token { get; set; }
    }
}