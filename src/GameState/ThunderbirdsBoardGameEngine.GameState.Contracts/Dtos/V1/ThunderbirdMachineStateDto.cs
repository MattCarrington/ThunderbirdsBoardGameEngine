namespace ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1
{
    public sealed record ThunderbirdMachineStateDto
    {
        public string ThunderbirdCode { get; init; } = string.Empty;

        public string LocationCode { get; init; } = string.Empty;

        public IReadOnlyList<string> OccupantCharacterCodes { get; init; } = [];
    }
}
