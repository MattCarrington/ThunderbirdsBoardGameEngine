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
                    Token = token
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
            token = default!;

            var trimmed = input.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return false;
            }

            if (EnumDictionary.RewardToken.TryGetValue(trimmed, out var output))
            {
                token = output;
                return true;
            }

            return false;
        }
    }
}
