using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SoundByte.Core.Items.Podcast
{
    [JsonObject]
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable once InconsistentNaming
    public partial class ITunesPodcast : IPodcast
    {
        [JsonProperty("trackId")]
        public int Id { get; set; }

        [JsonProperty("artistName")]
        public string Username { get; set; }

        [JsonProperty("trackName")]
        public string Title { get; set; }

        [JsonProperty("feedUrl")]
        public string FeedUrl { get; set; }

        [JsonProperty("artworkUrl600")]
        public string ArtworkUrl { get; set; }

        [JsonProperty("trackCount")]
        public int TrackCount { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime Created { get; set; }

        [JsonProperty("genres")]
        public List<string> Genres { get; set; }

        public BasePodcast ToBasePodcast()
        {
            return new BasePodcast
            {
                PodcastId = Id.ToString(),
                Username = Username,
                Title = Title,
                FeedUrl = FeedUrl,
                ArtworkUrl = ArtworkUrl,
                TrackCount = TrackCount,
                Created = Created,
                Genre = string.Join(",", Genres)
            };
        }
    }
}