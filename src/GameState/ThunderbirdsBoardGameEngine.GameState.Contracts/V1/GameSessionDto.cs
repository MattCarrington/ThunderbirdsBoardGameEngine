namespace ThunderbirdsBoardGameEngine.GameState.Contracts.V1
{
    public sealed record GameSessionDto
    {
        public Guid GameId { get; init; }

        public IReadOnlyCollection<ThunderbirdLocationDto> ThunderbirdLocation { get; init; } = Array.Empty<ThunderbirdLocationDto>();
    }
}
