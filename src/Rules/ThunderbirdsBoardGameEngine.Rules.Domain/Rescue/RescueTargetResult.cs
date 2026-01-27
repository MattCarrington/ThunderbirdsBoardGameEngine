namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public record RescueTargetResult
    {
        public required int TargetRoll { get; init; }

        public required int TotalBonus { get; init; }

        public IReadOnlyList<AppliedRescueModifier> AppliedBonuses { get; init; } = Array.Empty<AppliedRescueModifier>();
    }

    public record AppliedRescueModifier
    {
        public required string Key { get; init; }

        public required int Value { get; init; }

        public required string SourceType { get; init; } = "disaster-card"; // TODO: Currently, all bonuses come from disaster cards
    }
}
