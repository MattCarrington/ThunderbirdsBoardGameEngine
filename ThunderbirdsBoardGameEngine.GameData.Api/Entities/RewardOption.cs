using ThunderbirdsBoardGameEngine.GameData.Api.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Entities
{
    public class RewardOption
    {
        public bool IsUserChoice { get; set; }

        public IReadOnlyList<BonusTokens> BonusTokenChoices { get; set; } = Array.Empty<BonusTokens>();
    }
}
