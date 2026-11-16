namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records
{
    internal sealed class GameRecord
    {
        public Guid Id { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public string SetupVersion { get; set; } = null!;

        public long StateVersion { get; set; }

        public List<CharacterStateRecord> CharacterStates { get; set; } = [];

        public List<ThunderbirdMachineStateRecord> ThunderbirdMachineStates { get; set; } = [];

        public int ActiveTurnIndex { get; set; }

        public ICollection<GameTurnOrderEntryRecord> TurnOrderEntries { get; set; } = [];
    }
}
