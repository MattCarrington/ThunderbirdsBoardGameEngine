using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Mappers
{
    public static class RewardOptionCatalogDtoMapper
    {
        public static RewardOptionCatalogDto MapReward(string value)
        {
            if (IsBonusToken(value, out var token))
            {
                return new TokenRewardCatalogDto
                {
                    Token = StringHelpers.RemoveSpaces(token)
                };
            }

            if (string.Equals(value.Trim(), "User Choice", StringComparison.OrdinalIgnoreCase))
            {
                return new PlayerChoiceRewardCatalogDto();
            }

            throw new ArgumentException($"Unknown reward type: '{value}'");
        }

        private static bool IsBonusToken(string input, out string token)
        {
            token = input.Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var tokens = new[]
            {
                "Teamwork",
                "Intelligence",
                "Logistics",
                "Determination",
                "Technology"
            };

            return tokens.Contains(token, StringComparer.OrdinalIgnoreCase);
        }
    }
}
