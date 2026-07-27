namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    public sealed class GameStatePersistenceOptions
    {
        public const string SectionName = "GameState:Persistence";

        public string ConnectionString { get; set; } = string.Empty;
    }
}
