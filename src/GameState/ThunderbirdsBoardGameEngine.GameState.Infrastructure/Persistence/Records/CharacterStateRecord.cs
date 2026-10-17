namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records
{
    internal sealed class CharacterStateRecord
    {
        public Guid GameId { get; set; }

        public string CharacterCode { get; set; } = null!;

        public string ThunderbirdCode { get; set; } = null!;

        public GameRecord Game { get; set; } = null!;
    }
}
