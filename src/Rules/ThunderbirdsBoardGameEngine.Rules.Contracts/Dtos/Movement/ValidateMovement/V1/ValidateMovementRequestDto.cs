using System.ComponentModel.DataAnnotations;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Validation;

namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1
{
    /// <summary>
    /// Represents the data required to validate a movement action from a starting location to a destination location.
    /// </summary>
    public sealed record ValidateMovementRequestDto
    {
        /// <summary>
        /// Gets the unique key identifying the starting location of the movement action.
        /// </summary>
        [Required(ErrorMessage = "Start location must be provided.")]
        public required string StartLocation { get; init; }

        /// <summary>
        /// Gets the unique key identifying the destination location of the movement action.
        /// </summary>
        [Required(ErrorMessage = "Destination location must be provided.")]
        public required string DestinationLocation { get; init; }

        /// <summary>
        /// Gets the collection of unique keys for the Event Cards that are currently active.
        /// </summary>
        [NoBlankItems(ErrorMessage = "Active event card keys cannot contain null or whitespace values.")]
        public IReadOnlyCollection<string> ActiveEventCardKeys { get; init; } = [];
    }
}
