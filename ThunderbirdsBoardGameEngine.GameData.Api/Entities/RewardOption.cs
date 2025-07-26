using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Entities
{
    public class RewardOption
    {
        public bool IsUserChoice { get; set; }

        public BonusTokens? SpecifiedToken { get; set; }
    }
}
