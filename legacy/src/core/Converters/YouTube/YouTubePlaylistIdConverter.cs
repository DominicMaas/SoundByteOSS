using Newtonsoft.Json;
using SoundByte.Core.Items.YouTube;
using System;

namespace SoundByte.Core.Converters.YouTube
{
    /// <summary>
    ///     Convert a youtube playlist id (int) to an id object
    /// </summary>
    public class YouTubePlaylistIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(YouTubeId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new YouTubeId
                {
                    PlaylistId = (string)reader.Value,
                    Kind = "youtube#playlist"
                };
            }

            return serializer.Deserialize<YouTubeId>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}