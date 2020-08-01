using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SoundByte.Core.Models.Media
{
    /// <summary>
    ///     A base media item, used so the application can easily switch between different media
    ///     types in the same content group.
    /// </summary>
    public class Media : BaseModel
    {
        /// <summary>
        ///     The internal media type of this media object
        /// </summary>
        [JsonProperty("media_type")]
        public MediaType MediaType
        {
            get => _mediaType;
            set => Set(ref _mediaType, value);
        }

        private MediaType _mediaType;

        /// <summary>
        ///     The SoundByte resource ID
        /// </summary>
        [JsonProperty("id")]
        public Guid Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private Guid _id;

        /// <summary>
        ///     What service this media item belongs to. Useful for
        ///     performing service specific tasks such as liking.
        /// </summary>
        [JsonProperty("music_provider_id")]
        public Guid MusicProviderId
        {
            get => _musicProviderId;
            set => Set(ref _musicProviderId, value);
        }

        private Guid _musicProviderId;

        /// <summary>
        ///     Custom properties that can be
        /// </summary>
        [JsonProperty("custom_properties")]
        public Dictionary<string, object> CustomProperties { get; } = new Dictionary<string, object>();

        public Media(MediaType mediaType)
        {
            MediaType = mediaType;
        }
    }
}