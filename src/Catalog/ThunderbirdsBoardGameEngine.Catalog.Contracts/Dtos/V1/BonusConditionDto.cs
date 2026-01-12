namespace ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1
{
    /// <summary>
    /// A preformatted, display-only line for a single bonus condition as printed on the card.
    /// </summary>
    /// <remarks>
    /// Cards contain between 1 and 3 bonus conditions. Each condition may reduce the effective
    /// difficulty when the specified pieces are in the right place. This text is intended for UI
    /// rendering only; consumers must not parse it for rules.
    /// </remarks>
    public record BonusConditionDto
    {
        /// <summary>
        /// Represents the display-ready text shown on the physical card.
        /// This value is preformatted for presentation and should not be parsed.
        /// Examples: <c>"Virgil (+2)"</c>, <c>"Thunderbird 4 (+2) (if in Asia)"</c>.
        /// </summary>
        public string Description { get; init; } = string.Empty;

        /// <summary>
        /// Gets the unique identifier associated with this instance.
        /// </summary>
        public string Key { get; init; } = string.Empty;
    }
}