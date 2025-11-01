using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Parsers;

public static class RewardParser
{
    public static RewardOption Parse(string input)
    {
        input = input.Trim();

        if (string.Equals(input, "User Choice", StringComparison.OrdinalIgnoreCase))
        {
            return RewardOption.PlayerChoice();
        }

        // Normalize enum value
        if (Enum.TryParse<BonusToken>(NormalizeEnumName(input), ignoreCase: true, out var token))
        {
            return RewardOption.SpecifiedToken(token);
        }

        throw new ArgumentException($"Unknown reward type: '{input}'");
    }

    private static string NormalizeEnumName(string input)
    {
        return input.Replace(" ", "").Replace("-", "");
    }
}
