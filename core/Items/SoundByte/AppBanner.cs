using Newtonsoft.Json;

namespace SoundByte.Core.Items.SoundByte
{
    public class AppBanner
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string SubTitle { get; set; }

        [JsonProperty("button_text")]
        public string ButtonText { get; set; }

        [JsonProperty("button_link")]
        public string ButtonLink { get; set; }

        [JsonProperty("background_image")]
        public string BackgroundImage { get; set; }
    }
}