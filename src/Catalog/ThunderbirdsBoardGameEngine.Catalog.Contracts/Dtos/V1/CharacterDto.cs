namespace ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1
{
    /// <summary>
    /// Represents a data transfer object that contains basic information about a character.
    /// </summary>
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public record CharacterDto
    {
        /// <summary>
        /// Gets the unique identifier associated with this instance.
        /// </summary>
        public string Key { get; init; } = string.Empty;

        /// <summary>
        /// Gets the display name associated with the object.
        /// </summary>
        public string DisplayName { get; init; } = string.Empty;
    }
}
