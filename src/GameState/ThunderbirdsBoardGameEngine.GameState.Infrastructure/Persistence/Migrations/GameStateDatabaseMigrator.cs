using Microsoft.EntityFrameworkCore;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Migrations
{
    internal sealed class GameStateDatabaseMigrator : IGameStateDatabaseMigrator
    {
        private readonly GameStateDbContext _dbContext;

        public GameStateDatabaseMigrator(GameStateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Migrate(CancellationToken cancellationToken = default)
        {
            return _dbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}
