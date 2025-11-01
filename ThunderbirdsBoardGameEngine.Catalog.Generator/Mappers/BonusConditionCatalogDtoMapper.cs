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
                    Character = StringHelpers.RemoveSpaces(character),
                    BonusValue = value,
                    Location = MapLocation(location)
                };
            }

            if (IsThunderbirdMachine(target, out var thunderbird))
            {
                return new ThunderbirdBonusCatalogDto
                {
                    Thunderbird = StringHelpers.RemoveSpaces(thunderbird),
                    BonusValue = value,
                    Location = MapLocation(location)
                };
            }

            if (IsPodVehicle(target, out var podVehicle))
            {
                return new PodVehicleBonusCatalogDto
                {
                    PodVehicle = StringHelpers.RemoveSpaces(podVehicle),
                    BonusValue = value,
                    Location = MapLocation(location)
                };
            }

            throw new ArgumentException($"Unknown bonus target: '{target}'");
        }

        private static bool IsCharacter(string input, out string character)
        {
            character = input.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var characters = new[]
            {
                "Scott",
                "Virgil",
                "John",
                "Gordon",
                "Alan",
                "Lady Penelope"
            };

            return characters.Contains(character, StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsThunderbirdMachine(string input, out string thunderbird)
        {
            thunderbird = input.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            var thunderbirds = new[]
            {
                "Thunderbird 1",
                "Thunderbird 2",
                "Thunderbird 3",
                "Thunderbird 4",
                "Thunderbird 5",
                "Fab 1"
            };

            return thunderbirds.Contains(thunderbird, StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsPodVehicle(string input, out string podVehicle)
        {
            podVehicle = input.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            var podVehicles = new[]
            {
                "Mole",
                "DOMO",
                "Transmitter Truck",
                "Laser Cutter",
                "Elevator Cars",
                "Firefly",
                "Thunderizer",
                "Recovery Vehicles",
                "Mobile Crane",
                "Excavator"
            };

            return podVehicles.Contains(podVehicle, StringComparer.OrdinalIgnoreCase);
        }

        private static string? MapLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return null;
            }

            return StringHelpers.RemoveSpaces(location);
        }
    }
}
