using Microsoft.EntityFrameworkCore;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.EF.IntegrationTests.Persistence;

public sealed class GameRepositoryTests
{
    private const string ConnectionStringEnvironmentVariable =
        "GAME_STATE_TEST_CONNECTION_STRING";

    [Fact]
    public async Task SaveGameSession_ShouldPersistCompleteGame()
    {
        // Arrange
        var options = CreateDbContextOptions();
        var createdAtUtc = new DateTimeOffset(
            2026,
            7,
            28,
            12,
            0,
            0,
            TimeSpan.Zero);
        var game = new StandardGameSetupFactory()
            .Create(Guid.NewGuid(), createdAtUtc);

        try
        {
            await using (var writeContext = new GameStateDbContext(options))
            {
                var repository = new GameRepository(
                    writeContext,
                    new GameRecordMapper());

                // Act
                await repository.SaveGameSession(
                    game,
                    TestContext.Current.CancellationToken);
            }

            // Assert
            await using var readContext = new GameStateDbContext(options);

            var persistedGame = await readContext.Games
                .AsNoTracking()
                .Include(record => record.CharacterStates)
                .Include(record => record.ThunderbirdMachineStates)
                .SingleAsync(
                    record => record.Id == game.Id,
                    TestContext.Current.CancellationToken);

            Assert.Equal(game.Id, persistedGame.Id);
            Assert.Equal(game.CreatedAtUtc, persistedGame.CreatedAtUtc);
            Assert.Equal(game.SetupVersion, persistedGame.SetupVersion);

            Assert.Equal(
                game.Machines.Count,
                persistedGame.ThunderbirdMachineStates.Count);

            foreach (var expectedMachine in game.Machines)
            {
                var persistedMachine = Assert.Single(
                    persistedGame.ThunderbirdMachineStates,
                    record =>
                        record.ThunderbirdCode == expectedMachine.Key.Value);

                Assert.Equal(
                    expectedMachine.Value.Value,
                    persistedMachine.LocationCode);
            }

            Assert.Equal(
                game.Characters.Count,
                persistedGame.CharacterStates.Count);

            foreach (var expectedCharacter in game.Characters)
            {
                var persistedCharacter = Assert.Single(
                    persistedGame.CharacterStates,
                    record =>
                        record.CharacterCode == expectedCharacter.Key.Value);

                Assert.Equal(
                    expectedCharacter.Value.Value,
                    persistedCharacter.ThunderbirdCode);
            }
        }
        finally
        {
            await DeleteGame(options, game.Id);
        }
    }

    private static DbContextOptions<GameStateDbContext>
        CreateDbContextOptions()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            ConnectionStringEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"{ConnectionStringEnvironmentVariable} must contain the " +
                "PostgreSQL integration-test connection string.");
        }

        return new DbContextOptionsBuilder<GameStateDbContext>()
            .UseNpgsql(connectionString)
            .Options;
    }

    private static async Task DeleteGame(
        DbContextOptions<GameStateDbContext> options,
        Guid gameId)
    {
        await using var context = new GameStateDbContext(options);

        var persistedGame = await context.Games
            .SingleOrDefaultAsync(
                record => record.Id == gameId,
                CancellationToken.None);

        if (persistedGame is null)
        {
            return;
        }

        context.Games.Remove(persistedGame);
        await context.SaveChangesAsync(CancellationToken.None);
    }
}
