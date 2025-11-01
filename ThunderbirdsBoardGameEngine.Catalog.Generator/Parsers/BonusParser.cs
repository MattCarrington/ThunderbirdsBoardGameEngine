using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Parsers
{
    public static class BonusParser
    {
        public static BonusCondition Parse(string target, int value, string? location)
        {
            target = target.Trim();

            // Try as Character BonusCondition
            if (TryParseEnum<Character>(target, out var character))
            {
                return new CharacterBonusCondition
                (
                    character,
                    value,
                    ParseOptionalLocation(location)
                );
            }

            // Try as ThunderbirdMachine BonusCondition
            if (TryParseEnum<ThunderbirdMachine>(target, out var thunderbird))
            {
                return new ThunderbirdBonusCondition
                (
                    thunderbird,
                    value,
                    ParseOptionalLocation(location)
                );
            }

            // Try as PodVehicle BonusCondition
            if (TryParseEnum<PodVehicle>(target, out var podVehicle))
            {
                return new PodVehicleBonusCondition
                (
                    podVehicle,
                    value,
                    ParseOptionalLocation(location)
                );
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