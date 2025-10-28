using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Converters
{
    public class BonusConverter : JsonConverter<BonusCondition>
    {
        public override BonusCondition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                    return JsonSerializer.Deserialize<CharacterBonusCondition>(root.GetRawText(), options);
                case "thunderbirdBonus":
                    return JsonSerializer.Deserialize<ThunderbirdBonusCondition>(root.GetRawText(), options);
                case "podVehicleBonus":
                    return JsonSerializer.Deserialize<PodVehicleBonusCondition>(root.GetRawText(), options);
                default:
                    throw new JsonException($"Unknown Bonus type '{typeDiscriminator}'");
            }            
        }

        public override void Write(Utf8JsonWriter writer, BonusCondition value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            switch (value)
            {
                case CharacterBonusCondition cb:
                    writer.WriteString("type", "characterBonus");
                    writer.WriteNumber("bonusValue", cb.BonusValue);
                    writer.WriteString("character", JsonNamingPolicy.CamelCase.ConvertName(cb.Character.ToString()));
                    if (cb.Location != null)
                        writer.WriteString("location", JsonNamingPolicy.CamelCase.ConvertName(cb.Location.ToString()));
                    break;

                case ThunderbirdBonusCondition tb:
                    writer.WriteString("type", "thunderbirdBonus");
                    writer.WriteNumber("bonusValue", tb.BonusValue);
                    writer.WriteString("thunderbird", JsonNamingPolicy.CamelCase.ConvertName(tb.Thunderbird.ToString()));
                    if (tb.Location != null)
                        writer.WriteString("location", JsonNamingPolicy.CamelCase.ConvertName(tb.Location.ToString()));
                    break;

                case PodVehicleBonusCondition pb:
                    writer.WriteString("type", "podVehicleBonus");
                    writer.WriteNumber("bonusValue", pb.BonusValue);
                    writer.WriteString("podVehicle", JsonNamingPolicy.CamelCase.ConvertName(pb.PodVehicle.ToString()));
                    if (pb.Location != null)
                        writer.WriteString("location", JsonNamingPolicy.CamelCase.ConvertName(pb.Location.ToString()));
                    break;

                default:
                    throw new JsonException($"Unknown Bonus type '{value.GetType().Name}'");
            }

            writer.WriteEndObject();
        }
    }
}
