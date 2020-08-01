using Newtonsoft.Json;
using System;

namespace SoundByte.Core.Models.Media
{
    /// <summary>
    ///     Represents a track in the application
    /// </summary>
    public class Track : Media
    {
        public Track() : base(MediaType.Track)
        { }

        /// <summary>
        ///     Id of track
        /// </summary>
        [JsonProperty("track_id")]
        public string TrackId
        {
            get => _trackId;
            set => Set(ref _trackId, value);
        }

        private string _trackId;

        /// <summary>
        ///     Link to the track
        /// </summary>
        [JsonProperty("link")]
        public string Link
        {
            get => _link;
            set => Set(ref _link, value);
        }

        private string _link;

        /// <summary>
        ///     Is the track currently live
        /// </summary>
        [JsonProperty("is_live")]
        public bool IsLive
        {
            get => _isLive;
            set => Set(ref _isLive, value);
        }

        private bool _isLive;

        /// <summary>
        ///     Url to the audio stream
        /// </summary>
        [JsonProperty("audio_stream_url")]
        public string AudioStreamUrl
        {
            get => _audioStreamUrl;
            set => Set(ref _audioStreamUrl, value);
        }

        private string _audioStreamUrl;

        /// <summary>
        ///     Url to the video stream
        /// </summary>
        [JsonProperty("video_stream_url")]
        public string VideoStreamUrl
        {
            get => _videoStreamUrl;
            set => Set(ref _videoStreamUrl, value);
        }

        private string _videoStreamUrl;

        /// <summary>
        ///     Artwork link
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkUrl
        {
            get => _artworkUrl;
            set => Set(ref _artworkUrl, value);
        }

        private string _artworkUrl;

        /// <summary>
        ///     Title of the track
        /// </summary>
        [JsonProperty("title")]
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        private string _title;

        /// <summary>
        ///     Description of the track
        /// </summary>
        [JsonProperty("description")]
        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        private string _description;

        /// <summary>
        ///     How long is the track
        /// </summary>
        [JsonProperty("duration")]
        public TimeSpan Duration
        {
            get => _duration;
            set => Set(ref _duration, value);
        }

        private TimeSpan _duration;

        /// <summary>
        ///     Date and time the track was created/uploaded
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created
        {
            get => _created;
            set => Set(ref _created, value);
        }

        private DateTime _created;

        /// <summary>
        ///     The Track User
        /// </summary>
        [JsonProperty("user")]
        public User User
        {
            get => _user;
            set => Set(ref _user, value);
        }

        private User _user;
    }
}