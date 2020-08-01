using Newtonsoft.Json;

namespace SoundByte.Core.Models.Media
{
    /// <summary>
    ///     Represents a user in the application
    /// </summary>
    public class User : Media
    {
        public User() : base(MediaType.User)
        { }

        /// <summary>
        ///     Id of track
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId
        {
            get => _userId;
            set => Set(ref _userId, value);
        }

        private string _userId;

        [JsonProperty("username")]
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        private string _username;

        [JsonProperty("artwork_url")]
        public string ArtworkUrl
        {
            get => _artworkUrl;
            set => Set(ref _artworkUrl, value);
        }

        private string _artworkUrl;

        [JsonProperty("description")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private string _description;
    }
}