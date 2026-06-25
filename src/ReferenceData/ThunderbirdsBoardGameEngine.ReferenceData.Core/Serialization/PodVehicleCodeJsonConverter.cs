using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Serialization
{
    internal sealed class PodVehicleCodeJsonConverter : JsonConverter<PodVehicleCode>
    {
        public override PodVehicleCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"Expected JSON string when reading {nameof(PodVehicleCode)}, but found {reader.TokenType}.");
            }

            var value = reader.GetString();
            return new PodVehicleCode(value!);
        }

        public override void Write(Utf8JsonWriter writer, PodVehicleCode value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
