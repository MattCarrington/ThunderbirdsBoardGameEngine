namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1
{
    public record AppliedBonusDto
    {
        public required string BonusKey { get; init; }

        public required int BonusValue { get; init; }
    }
}
