using Newtonsoft.Json;
using SoundByte.Core.Items.YouTube;
using SoundByte.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SoundByte.Core.Items.User
{
    /// <summary>
    ///     A universal user class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BaseUser : BaseItem
    {
        /// <summary>
        ///     What service this user belongs to. Useful for
        ///     performing service specific tasks such as following.
        /// </summary>
        [JsonProperty("service_type")]
        public int ServiceType { get; set; }

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        ///     Link to the playlist
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; } = "https://soundbytemedia.com/pages/open-default-link";

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("track_count")]
        public double TrackCount { get; set; }

        [JsonProperty("followers_count")]
        public double FollowersCount { get; set; }

        [JsonProperty("followings_count")]
        public double FollowingsCount { get; set; }

        [JsonProperty("playlist_count")]
        public double PlaylistCount { get; set; }

        /// <summary>
        ///     Custom properties you can set
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();

        #region Static Methods

        public static async Task<BaseUser> GetUserAsync(int service, string userId)
        {
            switch (service)
            {
                case ServiceTypes.SoundCloud:
                case ServiceTypes.SoundCloudV2:
                    return (await SoundByteService.Current.GetAsync<SoundCloudUser>(ServiceTypes.SoundCloud, $"/users/{userId}")).Response.ToBaseUser();

                case ServiceTypes.YouTube:
                    var users = await SoundByteService.Current.GetAsync<YouTubeChannelHolder>(ServiceTypes.YouTube,
                        "channels",
                        new Dictionary<string, string>
                        {
                            {"part", "snippet,statistics"},
                            {"id", userId},
                        }).ConfigureAwait(false);

                    return users.Response.Channels.FirstOrDefault().ToBaseUser();

                default:
                    return null;
            }
        }

        #endregion Static Methods
    }
}