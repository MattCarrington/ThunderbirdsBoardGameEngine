namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    public record CalculateRescueTargetRequestDto
    {
        public required int CardId { get; init; }
        
        public required IReadOnlyCollection<string> AppliedBonusKeys { get; init; }
    }
}
