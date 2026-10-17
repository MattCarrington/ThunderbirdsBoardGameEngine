namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1
{
    /// <summary>
    /// Represents the runtime state of a Thunderbird machine, including its identifier, current location, and the list
    /// of occupant character codes.
    /// </summary>
    /// <remarks>Immutable DTO intended for serialization and inter-process communication. All properties are
    /// init-only; OccupantCharacterCodes is a read-only list that may be empty.</remarks>
    public sealed record ThunderbirdMachineStateDto
    {
        /// <summary>
        /// Gets the Thunderbird-specific code.
        /// </summary>
        /// <remarks>Init-only; immutable after object initialization. Non-null — defaults to an empty
        /// string.</remarks>
        public string ThunderbirdCode { get; init; } = string.Empty;

        /// <summary>
        /// Gets the current location code of the Thunderbird machine.
        /// </summary>
        /// <remarks>Init-only; immutable after object initialization. Non-null — defaults to an empty
        /// string.</remarks>
        public string LocationCode { get; init; } = string.Empty;

        /// <summary>
        /// Gets a read-only list of occupant character codes.
        /// </summary>
        /// <remarks>Initialized to an empty list. The property is init-only; each element is a string
        /// representing a character code used to identify an occupant.</remarks>
        public IReadOnlyList<string> OccupantCharacterCodes { get; init; } = [];
    }
}
