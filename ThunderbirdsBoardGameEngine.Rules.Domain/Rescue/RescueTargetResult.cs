namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public record RescueTargetResult
    {
        public required int TargetRoll { get; init; }

        public required int TotalBonus { get; init; }

        public IReadOnlyList<RescueBonus> AppliedBonuses { get; init; } = Array.Empty<RescueBonus>();
    }
}
