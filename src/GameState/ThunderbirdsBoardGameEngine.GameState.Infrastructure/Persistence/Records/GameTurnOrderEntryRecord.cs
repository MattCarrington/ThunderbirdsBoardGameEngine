namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records
{
    internal sealed class GameTurnOrderEntryRecord
    {
        public Guid GameId { get; set; }

        public int Position { get; set; }

        public string CharacterCode { get; set; } = null!;

        public GameRecord Game { get; set; } = null!;
    }
}
