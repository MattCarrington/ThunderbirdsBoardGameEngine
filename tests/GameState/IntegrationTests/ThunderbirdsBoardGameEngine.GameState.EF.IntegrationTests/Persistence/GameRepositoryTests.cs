using Microsoft.EntityFrameworkCore;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.EF.IntegrationTests.Persistence;

public sealed class GameRepositoryTests
{
    private const string ConnectionStringEnvironmentVariable =
        "GAME_STATE_TEST_CONNECTION_STRING";

    private static readonly DateTimeOffset CreatedAtUtc = new(
        2026,
        7,
        28,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public async Task SaveGameSession_ShouldPersistCompleteGame()
    {
        // Arrange
        var options = CreateDbContextOptions();

        var game = new StandardGameSetupFactory()
            .Create(Guid.NewGuid(), CreatedAtUtc);

        try
        {
            await using (var writeContext = new GameStateDbContext(options))
            {
                var repository = new GameRepository(
                    writeContext,
                    new GameRecordMapper());

                // Act
                await repository.CreateNewGameSession(
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

    [Fact]
    public async Task CreateAndGet_ShouldRoundTripCompleteGame()
    {
        // Arrange
        var options = CreateDbContextOptions();

        var game = new StandardGameSetupFactory()
            .Create(Guid.NewGuid(), CreatedAtUtc);

        try
        {
            await using (var writeContext = new GameStateDbContext(options))
            {
                var repository = new GameRepository(
                    writeContext,
                    new GameRecordMapper());

                await repository.CreateNewGameSession(
                    game,
                    TestContext.Current.CancellationToken);
            }

            await using var readContext = new GameStateDbContext(options);

            var repositoryToRestore = new GameRepository(
                readContext,
                new GameRecordMapper());

            // Act
            var restoredGame = await repositoryToRestore.GetGameSessionById(
                game.Id,
                TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(game.Id, restoredGame.Id);
            Assert.Equal(game.CreatedAtUtc, restoredGame.CreatedAtUtc);
            Assert.Equal(game.SetupVersion, restoredGame.SetupVersion);
            Assert.Equal(game.Machines.Count, restoredGame.Machines.Count);

            foreach (var expectedMachine in game.Machines)
            {
                Assert.True(
                    restoredGame.Machines.TryGetValue(
                        expectedMachine.Key,
                        out var restoredLocation));
                Assert.Equal(expectedMachine.Value, restoredLocation);

            }
            Assert.Equal(game.Characters.Count, restoredGame.Characters.Count);

            foreach (var expectedCharacter in game.Characters)
            {
                Assert.True(
                    restoredGame.Characters.TryGetValue(
                        expectedCharacter.Key,
                        out var restoredThunderbird));
                Assert.Equal(expectedCharacter.Value, restoredThunderbird);
            }
        }
        finally
        {
            await DeleteGame(options, game.Id);
        }
    }

    [Fact]
    public async Task GetGameSessionById_ShouldReturnNullForNonExistentGame()
    {
        // Arrange
        var options = CreateDbContextOptions();

        var nonExistentGameId = Guid.NewGuid();

        await using var context = new GameStateDbContext(options);

        var repository = new GameRepository(
            context,
            new GameRecordMapper());

        // Act
        var result = await repository.GetGameSessionById(
            nonExistentGameId,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateGameSession_ShouldPersistMovedThunderbird()
    {
        var options = CreateDbContextOptions();
        var game = new StandardGameSetupFactory().Create(
            Guid.NewGuid(),
            CreatedAtUtc);

        var destination = KnownLocationCodes.Europe;

        try
        {
            await using (var createContext =
                         new GameStateDbContext(options))
            {
                var repository = new GameRepository(
                    createContext,
                    new GameRecordMapper());

                await repository.CreateNewGameSession(
                    game,
                    TestContext.Current.CancellationToken);
            }

            await using (var updateContext =
                         new GameStateDbContext(options))
            {
                var repository = new GameRepository(
                    updateContext,
                    new GameRecordMapper());

                var gameToUpdate =
                    await repository.GetGameSessionById(
                        game.Id,
                        TestContext.Current.CancellationToken);

                Assert.NotNull(gameToUpdate);

                gameToUpdate.MoveThunderbirdMachine(
                    KnownThunderbirdCodes.Thunderbird1,
                    destination);

                await repository.UpdateGameSession(
                    gameToUpdate,
                    TestContext.Current.CancellationToken);
            }

            await using var verificationContext =
                new GameStateDbContext(options);

            var verificationRepository = new GameRepository(
                verificationContext,
                new GameRecordMapper());

            var restoredGame =
                await verificationRepository.GetGameSessionById(
                    game.Id,
                    TestContext.Current.CancellationToken);

            Assert.NotNull(restoredGame);

            Assert.Equal(
                destination,
                restoredGame.Machines[
                    KnownThunderbirdCodes.Thunderbird1]);

            Assert.Equal(game.Characters, restoredGame.Characters);
            Assert.Equal(game.CreatedAtUtc, restoredGame.CreatedAtUtc);
            Assert.Equal(game.SetupVersion, restoredGame.SetupVersion);
        }
        finally
        {
            await DeleteGame(options, game.Id);
        }
    }

    [Fact]
    public async Task UpdateSession_ShouldThrowWhenGameDoesNotExist()
    {
        // Arrange
        var options = CreateDbContextOptions();

        var nonExistentGameId = Guid.NewGuid();

        await using var context = new GameStateDbContext(options);

        var repository = new GameRepository(
            context,
            new GameRecordMapper());

        var nonExistentGame = new StandardGameSetupFactory()
            .Create(nonExistentGameId, CreatedAtUtc);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(async () =>
        {
            await repository.UpdateGameSession(
                nonExistentGame,
                TestContext.Current.CancellationToken);
        });
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
