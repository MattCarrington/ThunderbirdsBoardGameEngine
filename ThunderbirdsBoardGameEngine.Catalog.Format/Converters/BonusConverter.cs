using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Converters
{
    public class BonusConverter : JsonConverter<BonusCondition>
    {
        public override BonusCondition? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("type", out var typeProp))
                throw new JsonException("Missing discriminator 'type' for Bonus.");

            var type = typeProp.GetString();

            // shared fields
            if (!root.TryGetProperty("bonusValue", out var bonusProp))
                throw new JsonException("Missing 'bonusValue' for Bonus.");
            var bonusValue = bonusProp.GetInt32();

            BoardLocation? location = null;
            if (root.TryGetProperty("location", out var locProp))
            {
                if (!Enum.TryParse<BoardLocation>(locProp.GetString(), ignoreCase: true, out var loc))
                    throw new JsonException($"Invalid location '{locProp.GetString()}'.");
                location = loc;
            }

            switch (type)
            {
                case "characterBonus":
                    {
                        if (!root.TryGetProperty("character", out var charProp))
                            throw new JsonException("Missing 'character' for CharacterBonus.");
                        if (!Enum.TryParse<Character>(charProp.GetString(), ignoreCase: true, out var character))
                            throw new JsonException($"Invalid character '{charProp.GetString()}'.");
                        return new CharacterBonusCondition(character, bonusValue, location);
                    }
                case "thunderbirdBonus":
                    {
                        if (!root.TryGetProperty("thunderbird", out var tbProp))
                            throw new JsonException("Missing 'thunderbird' for ThunderbirdBonus.");
                        if (!Enum.TryParse<ThunderbirdMachine>(tbProp.GetString(), ignoreCase: true, out var thunderbird))
                            throw new JsonException($"Invalid thunderbird '{tbProp.GetString()}'.");
                        return new ThunderbirdBonusCondition(thunderbird, bonusValue, location);
                    }
                case "podVehicleBonus":
                    {
                        if (!root.TryGetProperty("podVehicle", out var pvProp))
                            throw new JsonException("Missing 'podVehicle' for PodVehicleBonus.");
                        if (!Enum.TryParse<PodVehicle>(pvProp.GetString(), ignoreCase: true, out var podVehicle))
                            throw new JsonException($"Invalid podVehicle '{pvProp.GetString()}'.");
                        return new PodVehicleBonusCondition(podVehicle, bonusValue, location);
                    }
                default:
                    throw new JsonException($"Unknown Bonus type '{type}'.");
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
