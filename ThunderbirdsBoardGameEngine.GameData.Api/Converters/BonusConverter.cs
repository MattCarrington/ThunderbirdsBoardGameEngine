using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Converters
{
    public class BonusConverter : JsonConverter<Bonus>
    {
        public override Bonus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;

            if (!root.TryGetProperty("type", out var typeProperty))
            {
                throw new JsonException("Missing discriminator 'type' for Bonus.");
            }

            var typeDiscriminator = typeProperty.GetString();

            switch (typeDiscriminator)
            {
                case "characterBonus":
                    return JsonSerializer.Deserialize<CharacterBonus>(root.GetRawText(), options);
                case "thunderbirdBonus":
                    return JsonSerializer.Deserialize<ThunderbirdBonus>(root.GetRawText(), options);
                case "podVehicleBonus":
                    return JsonSerializer.Deserialize<PodVehicleBonus>(root.GetRawText(), options);
                default:
                    throw new JsonException($"Unknown Bonus type '{typeDiscriminator}'");
            }            
        }

        public override void Write(Utf8JsonWriter writer, Bonus value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
