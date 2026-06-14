namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1
{
    /// <summary>
    /// DTO representing the response for a request to find accessible locations for a given Thunderbird. 
    /// It contains a collection of location codes that are accessible based on the Thunderbird's traversal domain.
    /// </summary>
    public record AccessibleLocationsResponseDto
    {
        /// <summary>
        /// Gets the collection of location codes that are accessible based on the Thunderbird's traversal domain.
        /// </summary>
        public IReadOnlyCollection<string> AccessibleLocations { get; init; } = Array.Empty<string>();
    }
}
