namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records
{
    internal sealed class ThunderbirdMachineStateRecord
    {
        public Guid GameId { get; set; }

        public string ThunderbirdCode { get; set; } = null!;

        public string LocationCode { get; set; } = null!;

        public GameRecord Game { get; set; } = null!;
    }
}
