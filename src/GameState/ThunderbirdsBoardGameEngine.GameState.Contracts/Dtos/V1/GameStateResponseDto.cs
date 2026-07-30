namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1
{
    public sealed record GameStateResponseDto
    {
        public Guid GameId { get; init; }

        public List<ThunderbirdMachineStateDto> ThunderbirdMachines { get; init; } = new();
    }
}
