using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Calculation
{
    public record RescueTargetResult
    {
        public required int TargetRoll { get; init; }

        public required int TotalBonus { get; init; }

        public IReadOnlyList<AppliedRescueModifier> AppliedModifiers { get; init; } = [];
    }
}
