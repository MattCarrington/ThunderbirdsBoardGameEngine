using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Entities
{
    public class RewardOption
    {
        public bool IsUserChoice { get; set; }

        public BonusToken? SpecifiedToken { get; set; }
    }
}
