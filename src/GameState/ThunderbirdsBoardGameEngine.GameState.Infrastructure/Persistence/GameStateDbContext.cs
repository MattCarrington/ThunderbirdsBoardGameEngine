using Microsoft.EntityFrameworkCore;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Records;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence
{
    internal class GameStateDbContext : DbContext
    {
        public GameStateDbContext(DbContextOptions<GameStateDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameRecord> Games => Set<GameRecord>();

        public DbSet<ThunderbirdMachineStateRecord> MachineStates => Set<ThunderbirdMachineStateRecord>();

        public DbSet<CharacterStateRecord> CharacterStates => Set<CharacterStateRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameStateDbContext).Assembly);
        }
    }
}
