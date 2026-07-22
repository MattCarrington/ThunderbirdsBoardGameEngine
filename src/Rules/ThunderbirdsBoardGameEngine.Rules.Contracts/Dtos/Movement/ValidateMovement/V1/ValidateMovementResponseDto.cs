namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1
{
    /// <summary>
    /// Represents the result of validating a movement action, including whether it is valid and any relevant messages or details about the validation outcome.
    /// </summary>
    public sealed record ValidateMovementResponseDto
    {
        /// <summary>
        /// Gets a value indicating whether the movement action is valid based on the game rules and current state.
        /// </summary>
        public required bool IsValid { get; init; }

        /// <summary>
        /// Gets the number of action points required to perform the movement action, if it is valid. 
        /// This value may be used by the client to determine if the performing character has sufficient resources to execute the movement.
        /// </summary>
        public required int ActionPointCost { get; init; }

        /// <summary>
        /// Gets the number of spaces travelled during the movement action.
        /// </summary>
        public required int SpacesTravelled { get; init; }

        /// <summary>
        /// Gets the top speed of the vehicle performing the movement action.
        /// </summary>
        public required int ThunderbirdTopSpeed { get; init; }

        /// <summary>
        /// Gets the effective top speed of the vehicle after applying any movement modifiers from event cards or other game effects.
        /// </summary>
        public required int? EffectiveTopSpeed { get; init; }

        /// <summary>
        /// Gets the route taken during the movement action, represented as a collection of location keys.
        /// </summary>
        public required IReadOnlyCollection<string> Route { get; init; }

        /// <summary>
        /// Gets an optional message providing additional information about the validation result, such as reasons for invalidity.
        /// </summary>
        public required IReadOnlyCollection<string> Messages { get; init; }
    }
}
