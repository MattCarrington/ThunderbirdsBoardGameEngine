namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record RuleBonus
    {
        public required string Key { get; init; }

        public required int Value { get; init; }
    }
}
