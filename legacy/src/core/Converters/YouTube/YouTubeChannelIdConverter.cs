using Newtonsoft.Json;
using SoundByte.Core.Items.YouTube;
using System;

namespace SoundByte.Core.Converters.YouTube
{
    /// <summary>
    ///     Convert a youtube channel id (int) to an id object
    /// </summary>
    public class YouTubeChannelIdConverter : JsonConverter
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
                    ChannelId = (string)reader.Value,
                    Kind = "youtube#channel"
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