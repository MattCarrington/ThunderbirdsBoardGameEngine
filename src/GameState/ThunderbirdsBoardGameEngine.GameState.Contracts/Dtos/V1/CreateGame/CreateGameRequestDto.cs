using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.CreateGame
{
    /// <summary>
    /// Request to create a game with a specified turn order.
    /// </summary>
    public record CreateGameRequestDto
    {
        /// <summary>
        /// Gets the order in which players take turns.
        /// </summary>
        /// <remarks>The list must contain between 2 and 4 entries.</remarks>
        [Required]
        [MinLength(2)]
        [MaxLength(4)]
        public required IReadOnlyList<string> TurnOrder { get; init; }
    }
}
