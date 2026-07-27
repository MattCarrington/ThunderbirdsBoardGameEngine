using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.Persistence.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.UnitTests.Configuration
{
    public class GameStatePersistenceOptionsValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValidResult_WhenOptionsAreValid()
        {
            // Arrange
            var options = new GameStatePersistenceOptions
            {
                ConnectionString = "Server=TestServer;Database=TestDb;User Id=testuser;Password=testpassword;"
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_ShouldReturnInvalidResult_WhenOptionsAreNull()
        {
            // Arrange
            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, null!);

            // Assert
            AssertFailureContains(result, "GameState persistence options are required.");
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Validate_ShouldReturnInvalidResult_WhenConnectionStringIsNullOrWhitespace(string? connectionString)
        {
            // Arrange
            var options = new GameStatePersistenceOptions
            {
                ConnectionString = connectionString!
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "GameState:Persistence:ConnectionString is required.");
        }

        [Fact]
        public void Validate_ShouldReturnInvalidResult_WhenConnectionStringDoesNotSpecifyHost()
        {
            // Arrange
            var options = new GameStatePersistenceOptions
            {
                ConnectionString = "Server=;Database=TestDb;User Id=testuser;Password=testpassword;"
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "GameState:Persistence:ConnectionString must specify a valid host.");
        }

        [Fact]
        public void Validate_ShouldReturnInvalidResult_WhenConnectionStringDoesNotSpecifyDatabase()
        {
            // Arrange
            var options = new GameStatePersistenceOptions
            {
                ConnectionString = "Server=localhost;User Id=testuser;Password=testpassword;"
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "GameState:Persistence:ConnectionString must specify a valid database name.");
        }

        [Fact]
        public void Validate_ShouldReturnInvalidResult_WhenConnectionStringIsMalformed()
        {
            // Arrange
            var options = new GameStatePersistenceOptions
            {
                ConnectionString = "InvalidConnectionString"
            };

            var validator = CreateValidator();

            // Act 
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "GameState:Persistence:ConnectionString is invalid.");
        }

        private static GameStatePersistenceOptionsValidator CreateValidator()
        {
            return new GameStatePersistenceOptionsValidator();
        }

        private static void AssertFailureContains(ValidateOptionsResult result, string expectedMessage)
        {
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failures);
            Assert.Contains(expectedMessage, result.Failures);
        }
    }
}
