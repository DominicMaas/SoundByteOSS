using Newtonsoft.Json;
using SoundByte.Core.Items.Track;
using SoundByte.Core.Items.User;
using System;
using System.Collections.Generic;

namespace SoundByte.Core.Items.Playlist
{
    [JsonObject]
    public class SoundCloudPlaylist : IPlaylist
    {
        /// <summary>
        ///     How long is the total playlists
        /// </summary>
        [JsonProperty("duration")]
        public double Duration { get; set; }

        /// <summary>
        ///     This title of this playlist
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     The genre of this playlist
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        ///     Used for updating the items within a users playlist
        /// </summary>
        [JsonProperty("secret_token")]
        public string SecretToken { get; set; }

        /// <summary>
        ///     The description for this track
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        ///     A list of track objects that are within this playlist
        /// </summary>
        [JsonProperty("tracks")]
        public List<SoundCloudTrack> Tracks { get; set; }

        /// <summary>
        ///     Internal ID of this object.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     The date and time of this objects creation
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     The artwork URI for this object
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkLink { get; set; }

        /// <summary>
        ///     The user who posted this track
        /// </summary>
        [JsonProperty("user")]
        public SoundCloudUser User { get; set; }

        /// <summary>
        ///     The number of likes that this playlist has
        /// </summary>
        [JsonProperty("likes_count")]
        public double? LikesCount { get; set; }

        /// <summary>
        ///     The number of tracks in this playlist
        /// </summary>
        [JsonProperty("track_count")]
        public double? TrackCount { get; set; }

        public BasePlaylist AsBasePlaylist => ToBasePlaylist();

        public BasePlaylist ToBasePlaylist()
        {
            var user = User.ToBaseUser();

            var thumbnailUrl = ArtworkLink;
            var artworkUrl = ArtworkLink;

            if (string.IsNullOrEmpty(ArtworkLink))
            {
                thumbnailUrl = user.ThumbnailUrl;
                artworkUrl = user.ArtworkUrl;
            }
            else if (ArtworkLink.Contains("large"))
            {
                thumbnailUrl = ArtworkLink.Replace("large", "t300x300");
                artworkUrl = ArtworkLink.Replace("large", "t500x500");
            }

            var basePlaylist = new BasePlaylist
            {
                ServiceType = ServiceTypes.SoundCloud,
                PlaylistId = Id,
                Duration = TimeSpan.FromMilliseconds(Duration),
                Title = Title,
                Genre = Genre,
                Description = Description,
                CreationDate = CreationDate,
                ArtworkUrl = artworkUrl,
                ThumbnailUrl = thumbnailUrl,
                User = User.ToBaseUser(),
                LikesCount = LikesCount ?? 0,
                TrackCount = TrackCount ?? 0
            };

            basePlaylist.CustomProperties.Add("SecretToken", SecretToken);

            return basePlaylist;
        }
    }
}