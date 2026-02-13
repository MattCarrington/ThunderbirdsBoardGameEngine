namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public record RescueTargetResult
    {
        public required int TargetRoll { get; init; }

        public required int TotalBonus { get; init; }

        public IReadOnlyList<AppliedRescueModifier> AppliedBonuses { get; init; } = [];
    }
}
