using Newtonsoft.Json;
using System;

namespace SoundByte.Core.Models.Media
{
    /// <summary>
    ///     Represents a playlist in the application
    /// </summary>
    public class Playlist : Media
    {
        public Playlist() : base(MediaType.Playlist)
        { }

        /// <summary>
        ///     Id of playlist
        /// </summary>
        [JsonProperty("playlist_id")]
        public string PlaylistId
        {
            get => _playlistId;
            set => Set(ref _playlistId, value);
        }

        private string _playlistId;

        /// <summary>
        ///     Link to the playlist
        /// </summary>
        [JsonProperty("link")]
        public string Link
        {
            get => _link;
            set => Set(ref _link, value);
        }

        private string _link;

        /// <summary>
        ///     Title of the playlist
        /// </summary>
        [JsonProperty("title")]
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _title;

        /// <summary>
        ///     Description of the playlist
        /// </summary>
        [JsonProperty("description")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private string _description;

        /// <summary>
        ///     How long is the playlist
        /// </summary>
        [JsonProperty("duration")]
        public TimeSpan Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

        private TimeSpan _duration;

        /// <summary>
        ///     Date and time the playlist was created/uploaded
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created
        {
            get => _created;
            set => Set(ref _created, value);
        }

        private DateTime _created;

        /// <summary>
        ///     The Playlist User
        /// </summary>
        [JsonProperty("user")]
        public User User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        private User _user;

        /// <summary>
        ///     Artwork for the playlist
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkUrl
        {
            get => _artworkUrl;
            set => Set(ref _artworkUrl, value);
        }

        private string _artworkUrl;

        /// <summary>
        ///     Used by SoundByte to determine if the track is in a playlist
        /// </summary>
        [JsonIgnore]
        public bool IsTrackInInternalPlaylist
        {
            get => _isTrackInInternalPlaylist;
            set => Set(ref _isTrackInInternalPlaylist, value);
        }

        private bool _isTrackInInternalPlaylist;
    }
}