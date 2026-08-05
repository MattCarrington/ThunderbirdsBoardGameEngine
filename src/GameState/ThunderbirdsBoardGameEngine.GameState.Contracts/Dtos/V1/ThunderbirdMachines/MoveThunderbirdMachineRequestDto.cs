using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines
{
    public record MoveThunderbirdMachineRequestDto
    {
        [Required]
        public required string Destination { get; init; }
    }
}
