namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1
{
    /// <summary>
    /// Represents the current state of a game, containing its unique identifier and the collection of Thunderbird
    /// machine states.
    /// </summary>
    /// <remarks>GameId uniquely identifies the game. ThunderbirdMachines is initialized to an empty list and
    /// will not be null. Properties use init-only setters.</remarks>
    public sealed record GameStateResponseDto
    {
        /// <summary>
        /// Gets the unique identifier for the game.
        /// </summary>
        /// <remarks>Assigned during object initialization and immutable thereafter. Must not be
        /// Guid.Empty.</remarks>
        public Guid GameId { get; init; }

        /// <summary>
        /// Gets the collection of Thunderbird machine states.
        /// </summary>
        /// <remarks>Initialized to an empty list and is init-only; the list reference cannot be replaced
        /// after initialization but its contents are mutable. Not safe for concurrent modification without
        /// synchronization.</remarks>
        public List<ThunderbirdMachineStateDto> ThunderbirdMachines { get; init; } = new();
    }
}
