namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines
{
    public record MoveThunderbirdMachineRequestDto
    {
        public string Destination { get; init; } = string.Empty;
    }
}
