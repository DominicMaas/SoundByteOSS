using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SoundByte.Core.Items.Podcast
{
    /// <summary>
    ///     A universal podcast class that is consistent for
    ///     all service types. All elements are updateable by
    ///     the UI.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class BasePodcast : BaseItem
    {
        /// <summary>
        ///     What service this podcast belongs to.
        /// </summary>
        [JsonProperty("service_type")]
        public int ServiceType { get; set; }

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Id of the podcast, useful for performing
        ///     tasks on the podcast.
        /// </summary>
        [JsonProperty("podcast_id")]
        public string PodcastId { get; set; }

        /// <summary>
        ///     Username of the person who uploaded the podcast.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        ///     Title of the podcast.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        ///     Url to the podcast feed.
        /// </summary>
        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        /// <summary>
        ///     Link to the podcast (internet)
        /// </summary>
        [JsonProperty("link")]
        public string Link { get; set; } = "https://soundbytemedia.com/pages/open-default-link";

        /// <summary>
        ///     Url to the artwork.
        /// </summary>
        [JsonProperty("artwork_url")]
        public string ArtworkUrl { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        /// <summary>
        ///     Number of tracks in the podcast.
        /// </summary>
        [JsonProperty("track_count")]
        public int TrackCount { get; set; }

        /// <summary>
        ///    Date and time this item was created.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        ///    Date and time this item was created.
        /// </summary>
        [JsonProperty("genre")]
        public string Genre { get; set; }

        /// <summary>
        ///     Custom properties you can set
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();
    }
}