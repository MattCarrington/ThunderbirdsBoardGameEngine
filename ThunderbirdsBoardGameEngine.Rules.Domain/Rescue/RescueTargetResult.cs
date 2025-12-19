namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public record RescueTargetResult
    {
        public required int TargetRoll { get; init; }

        public required int TotalBonus { get; init; }

        public IReadOnlyList<DisasterBonus> AppliedBonuses { get; init; } = Array.Empty<DisasterBonus>();
    }
}
