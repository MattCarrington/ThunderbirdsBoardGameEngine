using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests.Repositories
{
    public class DisasterCardRepositoryTests : IClassFixture<InfrastructureRegistrationFixture>
    {
        private readonly InfrastructureRegistrationFixture _fixture;
        private const string ConfigKey = "Catalog:DisasterCards:Json:FilePath";

        public DisasterCardRepositoryTests(InfrastructureRegistrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_WithValidFile_ReturnsCards()
        {
            // Arrange
            var filePath = TestDataPathHelper.GetPath("disaster-cards-test.json"); // non-empty valid JSON

            using var sp = _fixture.Build(ConfigKey, filePath);
            using var scope = sp.CreateScope();
            
            var repository = scope.ServiceProvider.GetRequiredService<IDisasterCardRepository>();

            // Act
            var cards = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(cards);
            Assert.NotEmpty(cards);
            Assert.Equal(2, cards.Count); // matches your test data
        }

        [Theory]
        [MemberData(nameof(InvalidFileCases))]
        public async Task GetAllAsync_WithInvalidFiles_ThrowsCatalogDataAccessException(string filename, CatalogDataAccessErrorCode expectedErrorCode)
        {
            // Arrange
            var filePath = TestDataPathHelper.GetPath(filename); // non-empty valid JSON

            using var sp = _fixture.Build(ConfigKey, filePath);
            using var scope = sp.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IDisasterCardRepository>();

            // Act & Assert: repo maps to CatalogDataAccessException.BadJson
            var ex = await Assert.ThrowsAsync<CatalogDataAccessException>(() => repository.GetAllAsync(CancellationToken.None));
            Assert.Equal(expectedErrorCode, ex.ErrorCode);
        }

        [Fact]
        public async Task GetAllAsync_WithInvalidDisasterCards_ThrowsDisasterCardValidationException()
        {
            // Arrange
            var filePath = TestDataPathHelper.GetPath("invalid-disaster-cards.json"); // syntactically invalid

            using var sp = _fixture.Build(ConfigKey, filePath);
            using var scope = sp.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IDisasterCardRepository>();

            // Act & Assert: repo maps to CatalogDataAccessException.BadJson
            var exception = await Assert.ThrowsAsync<DisasterCardValidationException>(() => repository.GetAllAsync(CancellationToken.None));
            Assert.Contains("Disaster Card Asteroid Impact must have at least one bonus condition.", exception.Message);
        }

        [Fact]
        public async Task GetAllAsync_WhenCancelledToken_ThrowsOperationCanceledException()
        {
            // Arrange
            var filePath = TestDataPathHelper.GetPath("disaster-cards-test.json"); // non-empty valid JSON

            using var sp = _fixture.Build(ConfigKey, filePath);
            using var scope = sp.CreateScope();

            var repository = scope.ServiceProvider.GetRequiredService<IDisasterCardRepository>();

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => repository.GetAllAsync(cts.Token));
        }

        public static IEnumerable<object[]> InvalidFileCases()
        {
            yield return new object[] {
                "invalid-json.json",
                CatalogDataAccessErrorCode.BadJson
            };
            yield return new object[] {
                "empty.json",
                CatalogDataAccessErrorCode.DataMissing                
            };
        }
    }
}
