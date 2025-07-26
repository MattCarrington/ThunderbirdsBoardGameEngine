using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Importer.Helpers;

namespace GameDataImporter.ConsoleApp.Parsers
{
    public static class BonusParser
    {
        public static Bonus Parse(string target, int value, string? location)
        {
            target = target.Trim();

            // Character Bonus
            if (Enum.TryParse<Character>(NormalizeEnumName(target), ignoreCase: true, out var character))
            {
                return new CharacterBonus
                {
                    Character = character,
                    BonusValue = value,
                    Location = string.IsNullOrWhiteSpace(location) ? null : EnumDisplayMapper.ParseLocation(location)
                };
            }

            // Thunderbird Bonus
            if (Enum.TryParse<Thunderbird>(NormalizeEnumName(target), ignoreCase: true, out var thunderbird))
            {
                return new ThunderbirdBonus
                {
                    Thunderbird = thunderbird,
                    BonusValue = value,
                    Location = string.IsNullOrWhiteSpace(location) ? null : EnumDisplayMapper.ParseLocation(location)
                };
            }

            // PodVehicle Bonus
            if (Enum.TryParse<PodVehicle>(NormalizeEnumName(target), ignoreCase: true, out var podVehicle))
            {
                return new PodVehicleBonus
                {
                    PodVehicle = podVehicle,
                    BonusValue = value,
                    Location = string.IsNullOrWhiteSpace(location) ? null : EnumDisplayMapper.ParseLocation(location)
                };
            }

            throw new ArgumentException($"Unknown bonus target: '{target}'");
        }

        private static string NormalizeEnumName(string input)
        {
            return input.Replace(" ", "").Replace("-", "");
        }
    }
}
