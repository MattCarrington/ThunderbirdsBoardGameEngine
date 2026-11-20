using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame
{
    /// <summary>
    /// Request to create a game with a specified turn order.
    /// </summary>
    public record CreateGameRequestDto : IValidatableObject
    {
        /// <summary>
        /// Gets the order in which players take turns.
        /// </summary>
        /// <remarks>The list must contain between 2 and 4 entries.</remarks>
        [Required(ErrorMessage = "Turn order must be provided.")]
        [MinLength(2, ErrorMessage = "Turn order must contain at least two characters.")]
        [MaxLength(4, ErrorMessage = "Turn order must contain no more than four characters.")]
        public required IReadOnlyList<string> TurnOrder { get; init; }

        /// <summary>
        /// Validates that every turn-order character code is provided.
        /// </summary>
        /// <param name="validationContext">The context in which the validation is performed.</param>
        /// <returns>Validation errors for missing turn-order character codes; otherwise, an empty sequence.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TurnOrder is null)
            {
                yield break;
            }

            if (TurnOrder.Any(string.IsNullOrWhiteSpace))
            {
                yield return new ValidationResult(
                    "Every turn-order character code must be provided.",
                    [nameof(TurnOrder)]);
            }
        }
    }
}
