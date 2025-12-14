using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Mappers
{
    public static class BonusConditionCatalogDtoMapper
    {
        public static BonusConditionCatalogDto MapBonus(string target, int value, string? location)
        {
            // Try as Character BonusCondition
            if (IsCharacter(target, out var character))
            {
                return new CharacterBonusCatalogDto
                {
                    Character = character,
                    BonusValue = value,
                    Location = MapLocation(location)
                };
            }

            if (IsThunderbirdMachine(target, out var thunderbird))
            {
                return new ThunderbirdBonusCatalogDto
                {
                    Thunderbird = thunderbird,
                    BonusValue = value,
                    Location = MapLocation(location)
                };
            }

            if (IsPodVehicle(target, out var podVehicle))
            {
                return new PodVehicleBonusCatalogDto
                {
                    PodVehicle = podVehicle,
                    BonusValue = value,
                    Location = MapLocation(location)
                };
            }

            throw new ArgumentException($"Unknown bonus target: '{target}'");
        }

        private static bool IsCharacter(string input, out string character)
        {
            character = default!;

            var trimmed = input.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return false;
            }

            if (EnumDictionary.Character.TryGetValue(trimmed, out var output))
            {
                character = output;
                return true;
            }

            return false;
        }

        private static bool IsThunderbirdMachine(string input, out string thunderbird)
        {
            thunderbird = default!;

            var trimmed = input.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return false;
            }

            if (EnumDictionary. Thunderbird.TryGetValue(trimmed, out var output))
            {
                thunderbird = output;
                return true;
            }

            return false;
        }

        private static bool IsPodVehicle(string input, out string podVehicle)
        {
            podVehicle = default!;

            var trimmed = input.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                podVehicle = default!;
                return false;
            }

            if (EnumDictionary.PodVehicle.TryGetValue(trimmed, out var output))
            {
                podVehicle = output;
                return true;
            }

            return false;
        }

        private static string? MapLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return null;
            }

            return LocationMapper.MapLocation(location);
        }
    }
}
