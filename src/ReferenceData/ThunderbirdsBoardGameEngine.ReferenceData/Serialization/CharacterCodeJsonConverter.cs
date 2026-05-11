using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Serialization
{
    internal sealed class CharacterCodeJsonConverter : JsonConverter<CharacterCode>
    {
        public override CharacterCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string when reading {nameof(CharacterCode)}, but found {reader.TokenType}.");
            }

            var value = reader.GetString();
            return new CharacterCode(value!);
        }

        public override void Write(Utf8JsonWriter writer, CharacterCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
