using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace GameDataImporter.ConsoleApp.Parsers;

public static class RewardParser
{
    public static RewardOption Parse(string input)
    {
        input = input.Trim();

        if (string.Equals(input, "User Choice", StringComparison.OrdinalIgnoreCase))
        {
            return new RewardOption
            {
                IsUserChoice = true
            };
        }

        // Normalize enum value
        if (Enum.TryParse<BonusToken>(NormalizeEnumName(input), ignoreCase: true, out var token))
        {
            return new RewardOption
            {
                IsUserChoice = false,
                SpecifiedToken = token
            };
        }

        throw new ArgumentException($"Unknown reward type: '{input}'");
    }

    private static string NormalizeEnumName(string input)
    {
        return input.Replace(" ", "").Replace("-", "");
    }
}
