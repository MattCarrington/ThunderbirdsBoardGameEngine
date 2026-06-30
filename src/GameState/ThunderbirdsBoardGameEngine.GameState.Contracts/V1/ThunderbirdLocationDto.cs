namespace ThunderbirdsBoardGameEngine.GameState.Contracts.V1
{
    public sealed record ThunderbirdLocationDto
    {
        public string ThunderbirdCode { get; init; } = string.Empty;

        public string LocationCode { get; init; } = string.Empty;
    }
}
