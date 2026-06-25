using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Serialization
{
    internal sealed class DisasterBonusKeyJsonConverter : JsonConverter<DisasterBonusKey>
    {
        public override DisasterBonusKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string when reading {nameof(DisasterBonusKey)}, but found {reader.TokenType}.");
            }

            var value = reader.GetString();
            return new DisasterBonusKey(value!);
        }

        public override void Write(Utf8JsonWriter writer, DisasterBonusKey value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
