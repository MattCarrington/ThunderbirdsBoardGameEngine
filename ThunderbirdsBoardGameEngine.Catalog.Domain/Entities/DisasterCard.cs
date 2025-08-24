using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public class DisasterCard
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DifficultyNumber { get; set; }

        public BoardLocation Location { get; set; }

        public RescueType RescueType { get; set; }

        public IReadOnlyList<BonusCondition> BonusConditions { get; set; } = Array.Empty<BonusCondition>();

        public IReadOnlyList<RewardOption> RewardOptions { get; set; } = Array.Empty<RewardOption>();
    }
}
