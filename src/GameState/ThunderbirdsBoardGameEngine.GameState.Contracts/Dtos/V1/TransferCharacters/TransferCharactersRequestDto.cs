using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters
{
    /// <summary>
    /// Represents a request to transfer one or more characters by specifying transfer assignments.
    /// </summary>
    /// <remarks>Requires at least one assignment and supports up to two assignments.</remarks>
    public record TransferCharactersRequestDto
    {
        /// <summary>
        /// Character transfer assignments.
        /// </summary>
        [Required]
        [MinLength(1)]
        [MaxLength(2)]
        public required IReadOnlyList<CharacterTransferAssignmentDto> Assignments { get; init; }
    }
}
