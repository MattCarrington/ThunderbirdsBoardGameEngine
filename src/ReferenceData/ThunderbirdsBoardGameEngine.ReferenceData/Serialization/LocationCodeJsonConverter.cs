using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Serialization
{
    public class LocationCodeJsonConverter : JsonConverter<LocationCode>
    {
        public override LocationCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string when reading {nameof(LocationCode)}, but found {reader.TokenType}.");
            }

            var value = reader.GetString();
            return new LocationCode(value!);
        }

        public override void Write(Utf8JsonWriter writer, LocationCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
