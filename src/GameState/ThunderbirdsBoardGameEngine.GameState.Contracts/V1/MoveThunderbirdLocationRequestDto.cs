namespace ThunderbirdsBoardGameEngine.GameState.Contracts.V1
{
    public sealed record MoveThunderbirdLocationRequestDto
    {
        public string ThunderbirdCode { get; init; } = string.Empty;

        public string Destination { get; init; } = string.Empty;
    }
}
