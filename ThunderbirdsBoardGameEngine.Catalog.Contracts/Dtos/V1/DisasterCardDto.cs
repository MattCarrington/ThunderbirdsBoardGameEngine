namespace ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1
{
    /// <summary>
    /// A Disaster Card exposed by the Catalog API.
    /// </summary>
    /// /// <remarks>
    /// All string and collection properties are non-null and preformatted for UI display.
    /// Consumers must not parse or modify these values.
    /// </remarks>
    public record DisasterCardDto
    {
        /// <summary>
        /// Unique identifier for the card.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// The title as displayed on the card.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// The difficulty number as displayed on the card.
        /// </summary>
        public int DifficultyNumber { get; init; }

        /// <summary>
        /// The location of the disaster as displayed on the card.
        /// </summary>
        /// <remarks>
        /// Represents the area of the board where the rescue occurs.
        /// This is a display-only value and must not be parsed or used for movement logic.
        /// Possible values correspond to the locations printed on the physical board, such as
        /// <c>Europe</c>, <c>Asia</c>, <c>North Pacific</c>, <c>Geo-Stationary Orbit</c>, and so on.
        /// </remarks>
        public string Location { get; init; } = string.Empty;

        /// <summary>
        /// The rescue type as displayed on the card.
        /// </summary>
        /// <remarks>
        /// One of the following exact values: <c>Land</c>, <c>Sea</c>, <c>Air</c>, <c>Space</c>.
        /// This is a display-only value and must not be parsed.
        /// </remarks>
        public string RescueType { get; init; } = string.Empty;

        /// <summary>
        /// Display-only bonus conditions for this card. Never null; empty when none.
        /// </summary>
        /// <remarks>
        /// The list will contain between 1 and 3 items for standard cards. Order matches the physical card.
        /// Each item is preformatted text and must not be parsed.
        /// </remarks>
        public IReadOnlyList<BonusConditionDto> BonusConditions { get; init; } = [];

        /// <summary>
        /// Rewards printed on the card, in card order. Never null; empty when none.
        /// </summary>
        /// <remarks>
        /// Contains one or two items. Each item’s <c>DisplayName</c> is exactly one of:
        /// "Teamwork", "Determination", "Logistics", "Intelligence", "Technology", or "Player Choice".
        /// </remarks>
        public IReadOnlyList<RewardDto> Rewards { get; init; } = [];
    }
}
