namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public record AppliedRescueModifier
    {
        public required string Key { get; init; }

        public required int Value { get; init; }

        public required SourceType SourceType { get; init; }
    }

    public enum SourceType
    {
        DisasterCard,
        CharacterAbility,
        FabCard,
        EventCard
    }
}
