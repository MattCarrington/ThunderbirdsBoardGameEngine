using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.EndTurn
{
    /// <summary>
    /// Represents a request to end the active character's turn.
    /// </summary>
    public record EndTurnRequestDto
    {
        /// <summary>
        /// Gets or sets the key of the character whose turn is ending.
        /// </summary>
        [Required]
        public string ExpectedActiveCharacterCode { get; init; } = string.Empty;
    }
}
