using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.Serialization.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Importer.Parsers
{
    public static class BonusParser
    {
        public static Bonus Parse(string target, int value, string? location)
        {
            target = target.Trim();

            // Try as Character Bonus
            if (TryParseEnum<Character>(target, out var character))
            {
                return new CharacterBonus
                {
                    Character = character,
                    BonusValue = value,
                    Location = ParseOptionalLocation(location)
                };
            }

            // Try as ThunderbirdMachine Bonus
            if (TryParseEnum<ThunderbirdMachine>(target, out var thunderbird))
            {
                return new ThunderbirdBonus
                {
                    Thunderbird = thunderbird,
                    BonusValue = value,
                    Location = ParseOptionalLocation(location)
                };
            }

            // Try as PodVehicle Bonus
            if (TryParseEnum<PodVehicle>(target, out var podVehicle))
            {
                return new PodVehicleBonus
                {
                    PodVehicle = podVehicle,
                    BonusValue = value,
                    Location = ParseOptionalLocation(location)
                };
            }

            throw new ArgumentException($"Unknown bonus target: '{target}'");
        }

        private static BoardLocation? ParseOptionalLocation(string? loc) =>
            string.IsNullOrWhiteSpace(loc) ? null : EnumDisplayHelper.ParseFromDisplayName<BoardLocation>(loc);

        private static bool TryParseEnum<TEnum>(string input, out TEnum result) where TEnum : struct, Enum
        {
            try
            {
                result = EnumDisplayHelper.ParseFromDisplayName<TEnum>(input);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}