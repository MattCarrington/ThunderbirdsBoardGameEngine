namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines
{
    public record MoveThunderbirdMachineRequestDto
    {
        public required string Destination { get; init; }
    }
}
