namespace ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1
{
    /// <summary>
    /// A single reward printed on the card, represented as preformatted text.
    /// </summary>
    /// <remarks>
    /// Allowed values (exact, case-sensitive): "Teamwork", "Determination", "Logistics",
    /// "Intelligence", "Technology", or "Player Choice".
    /// "Player Choice" means the player may choose one of the five tokens above.
    /// This DTO is presentation-only; consumers MUST NOT parse or infer rules beyond the values listed.
    /// </remarks>
    public record RewardDto
    {
        /// <summary>
        /// Reward text to show to the player. Never null; empty when unspecified.
        /// One of: "Teamwork", "Determination", "Logistics", "Intelligence", "Technology", "Player Choice".
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;
    }
}