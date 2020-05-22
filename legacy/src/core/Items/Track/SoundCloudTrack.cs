using Newtonsoft.Json;
using SoundByte.Core.Items.User;
using SoundByte.Core.Services;
using System;
using System.Threading.Tasks;

namespace SoundByte.Core.Items.Track
{
    /// <summary>
    ///     SoundCloud Specific Item
    /// </summary>
    [JsonObject]
    public class SoundCloudTrack : ITrack
    {
        public SoundCloudTrack()
        {
        }

        public SoundCloudTrack(string id)
        {
            Id = int.Parse(id);
        }

        [JsonProperty("artwork_url")]
        // ReSharper disable once InconsistentNaming
        private string _artworkUrl { get; set; }

        public string ArtworkUrl
        {
            set => _artworkUrl = value;
            get
            {
                if (string.IsNullOrEmpty(_artworkUrl))
                {
                    return User.AvatarUrl;
                }
                else
                {
                    return _artworkUrl;
                }
            }
        }

        [JsonProperty("commentable")]
        public bool IsCommentable { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("download_count")]
        public int DownloadCount { get; set; }

        [JsonProperty("download_url")]
        public string DownloadUrl { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("license")]
        public string License { get; set; }

        [JsonProperty("likes_count")]
        public int LikesCount { get; set; }

        [JsonProperty("permalink")]
        public string Permalink { get; set; }

        [JsonProperty("permalink_url")]
        public string PermalinkUrl { get; set; }

        [JsonProperty("playback_count")]
        public int PlaybackCount { get; set; }

        [JsonProperty("@public")]
        public bool IsPublic { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("reposts_count")]
        public int RepostsCount { get; set; }

        [JsonProperty("secret_token")]
        public object SecretToken { get; set; }

        [JsonProperty("tag_list")]
        public string TagList { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("user")]
        public SoundCloudUser User { get; set; }

        [JsonProperty("user_favorite")]
        public bool? IsUserLiked { get; set; }

        public BaseTrack AsBaseTrack => ToBaseTrack();

        /// <summary>
        ///     Convert this SoundCloud specific track to a universal track.
        /// </summary>
        /// <returns></returns>
        public BaseTrack ToBaseTrack()
        {
            var user = User.ToBaseUser();

            var thumbnailUrl = ArtworkUrl;
            var artworkUrl = ArtworkUrl;

            if (string.IsNullOrEmpty(ArtworkUrl))
            {
                thumbnailUrl = user.ThumbnailUrl;
                artworkUrl = user.ArtworkUrl;
            }
            else if (ArtworkUrl.Contains("large"))
            {
                thumbnailUrl = ArtworkUrl.Replace("large", "t300x300");
                artworkUrl = ArtworkUrl.Replace("large", "t500x500");
            }

            return new BaseTrack
            {
                ServiceType = ServiceTypes.SoundCloud,
                TrackId = Id.ToString(),
                Link = PermalinkUrl,
                AudioStreamUrl = string.Empty,
                VideoStreamUrl = string.Empty,
                ArtworkUrl = artworkUrl,
                ThumbnailUrl = thumbnailUrl,
                Title = Title,
                Description = Description,
                Duration = TimeSpan.FromMilliseconds(Duration),
                Created = DateTime.Parse(CreatedAt),
                LikeCount = LikesCount,
                DislikeCount = 0,
                ViewCount = PlaybackCount,
                CommentCount = CommentCount,
                Genre = Genre,
                IsLiked = IsUserLiked.HasValue && IsUserLiked.Value,
                User = user
            };
        }

        public async Task<bool> LikeAsync()
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                return false;

            return (await SoundByteService.Current.PutAsync(ServiceTypes.SoundCloud,
                $"/e1/me/track_likes/{Id}")).Response;
        }

        public async Task<bool> UnlikeAsync()
        {
            if (!SoundByteService.Current.IsServiceConnected(ServiceTypes.SoundCloud))
                return false;

            return (await SoundByteService.Current.DeleteAsync(ServiceTypes.SoundCloud,
                $"/e1/me/track_likes/{Id}")).Response;
        }
    }
}