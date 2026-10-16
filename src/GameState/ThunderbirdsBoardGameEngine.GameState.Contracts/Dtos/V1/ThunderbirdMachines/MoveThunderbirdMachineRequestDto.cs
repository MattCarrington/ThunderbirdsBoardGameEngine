using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines
{
    /// <summary>
    /// Request DTO that specifies the target destination for moving a Thunderbird machine.
    /// </summary>
    /// <remarks>Destination is required and must identify the target location (for example, host, path, or
    /// node identifier). Validation and move execution occur on the server.</remarks>
    public record MoveThunderbirdMachineRequestDto
    {
        /// <summary>
        /// Gets the destination to which data or output will be sent.
        /// </summary>
        /// <remarks>Required; must be provided during object initialization and must not be null or
        /// empty.</remarks>
        [Required]
        public required string Destination { get; init; }
    }
}
