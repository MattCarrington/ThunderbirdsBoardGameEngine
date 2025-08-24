using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public class RewardOption
    {
        public bool IsUserChoice { get; set; }

        public BonusToken? SpecifiedToken { get; set; }
    }
}
