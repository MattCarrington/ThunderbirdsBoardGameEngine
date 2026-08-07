namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations
{
    public interface IGameStateDatabaseMigrator
    {
        Task Migrate(CancellationToken cancellationToken = default);
    }
}