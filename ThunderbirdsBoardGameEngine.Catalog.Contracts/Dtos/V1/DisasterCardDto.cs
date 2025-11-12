namespace ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1
{
    public record DisasterCardDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int DifficultyNumber { get; set; }

        public string Location { get; set; } = string.Empty;

        public string RescueType { get; set; } = string.Empty;

        public IReadOnlyList<BonusConditionDto> BonusConditions { get; set; } = [];

        public IReadOnlyList<RewardDto> Rewards { get; set; } = [];
    }
}
