using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Serialization
{
    public class ThunderbirdCodeJsonConverter : JsonConverter<ThunderbirdCode>
    {
        public override ThunderbirdCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string when reading {nameof(ThunderbirdCode)}, but found {reader.TokenType}.");
            }

            var value = reader.GetString();
            return new ThunderbirdCode(value!);
        }

        public override void Write(Utf8JsonWriter writer, ThunderbirdCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
