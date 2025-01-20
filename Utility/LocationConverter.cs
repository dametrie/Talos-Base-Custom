using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Talos.Structs;

namespace Talos.Utility
{
    internal class LocationConverter : JsonConverter<Location>
    {
        public override Location ReadJson(
            JsonReader reader,
            Type objectType,
            Location existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                // If JSON is null, return a default Location
                return new Location();
            }

            // Load the JObject
            var jObject = JObject.Load(reader);

            // Safely extract fields (default to 0 if null)
            short mapId = (short)(jObject["MapId"]?.Value<int>() ?? 0);
            short x = (short)(jObject["X"]?.Value<int>() ?? 0);
            short y = (short)(jObject["Y"]?.Value<int>() ?? 0);

            return new Location(mapId, x, y);
        }

        public override void WriteJson(JsonWriter writer, Location value, JsonSerializer serializer)
        {
            // Write out the Location object
            writer.WriteStartObject();
            writer.WritePropertyName("MapId");
            writer.WriteValue(value.MapID);
            writer.WritePropertyName("X");
            writer.WriteValue(value.X);
            writer.WritePropertyName("Y");
            writer.WriteValue(value.Y);
            writer.WriteEndObject();
        }
    }
}
