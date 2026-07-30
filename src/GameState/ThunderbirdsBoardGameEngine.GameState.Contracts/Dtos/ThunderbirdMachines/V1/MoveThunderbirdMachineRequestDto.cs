namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.ThunderbirdMachines.V1
{
    public record MoveThunderbirdMachineRequestDto
    {
        public string Destination { get; init; } = string.Empty;
    }
}
