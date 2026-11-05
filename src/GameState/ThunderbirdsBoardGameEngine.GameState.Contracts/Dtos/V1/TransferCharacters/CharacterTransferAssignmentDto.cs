using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.TransferCharacters
{
    /// <summary>
    /// Represents a data transfer object that assigns a character to a destination Thunderbird code for transfer.
    /// </summary>
    public record CharacterTransferAssignmentDto
    {
        /// <summary>
        /// Gets the character code.
        /// </summary>
        /// <remarks>Required. Set during initialiszation.</remarks>
        [Required]
        public required string CharacterCode { get; init; }

        /// <summary>
        /// Gets the Thunderbird code that identifies the destination.
        /// </summary>
        /// <remarks>This value is required.</remarks>
        [Required]
        public required string DestinationThunderbirdCode { get; init; }
    }
}
