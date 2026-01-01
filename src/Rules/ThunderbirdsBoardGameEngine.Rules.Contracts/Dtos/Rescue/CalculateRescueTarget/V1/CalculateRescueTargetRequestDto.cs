namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    public record CalculateRescueTargetRequestDto
    {
        public required IReadOnlyCollection<string> PresentBonusKeys { get; init; }
    }
}
