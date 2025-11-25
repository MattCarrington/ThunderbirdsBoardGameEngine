using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests.Repositories
{
    public class DisasterCardReaderTests : IClassFixture<InfrastructureRegistrationFixture>
    {
        private readonly InfrastructureRegistrationFixture _fixture;
        private const string ConfigKey = "Catalog:DisasterCards:Json:FilePath";

        public DisasterCardReaderTests(InfrastructureRegistrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_WithValidFile_ReturnsCards()
        {
            // Arrange
            var filepath = EnvelopArray("disaster-cards-test.json"); // syntactically valid

            using var provider = _fixture.Build(ConfigKey, filepath);
            
            var reader = provider.GetRequiredService<IDisasterCardReader>();

            // Act
            var cards = await reader.GetAllAsync(CancellationToken.None);

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
            var filePath = DisasterCardTestFileCatalog.Invalid(filename); // non-empty valid JSON

            using var provider = _fixture.Build(ConfigKey, filePath);
            
            var reader = provider.GetRequiredService<IDisasterCardReader>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CatalogDataAccessException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Equal(expectedErrorCode, ex.ErrorCode);
        }

        [Fact]
        public async Task GetAllyAsync_WithEmptySpaceFile_ThrowsDataMissingException()
        {
            // Arrange
            var path = Path.Combine(Path.GetTempPath(),
                $"empty-{Path.GetRandomFileName()}.json");

            await File.WriteAllTextAsync(path, new string(' ', 25 * 1024 * 1024));

            using var provider = _fixture.Build(ConfigKey, path);

            var reader = provider.GetRequiredService<IDisasterCardReader>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Equal(CatalogDataAccessErrorCode.DataMissing, exception.ErrorCode);
        }

        [Fact]
        public async Task GetAllAsync_WhenFileUnreadable_ThrowsSourceUnreadableException()
        {
            // Arrange
            var path = Path.Combine(Path.GetTempPath(),
                $"unreadable-{Path.GetRandomFileName()}.json");

            await File.WriteAllTextAsync(path, "{ \"x\": 1 }");

            using var _ = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            using var provider = _fixture.Build(ConfigKey, path);

            var reader = provider.GetRequiredService<IDisasterCardReader>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Equal(CatalogDataAccessErrorCode.SourceUnreadable, exception.ErrorCode);
        }

        [Fact]
        public async Task GetAllAsync_WithInvalidDisasterCards_ThrowsDisasterCardValidationException()
        {
            // Arrange
            //var filepath = EnvelopArray("invalid-disaster-cards.json"); // syntactically invalid
            var bare = DisasterCardTestFileCatalog.Invalid("invalid-disaster-cards.json");
            var enveloped = TestJsonEnvelopeCreator.EnvelopArrayFile(bare);

            using var provider = _fixture.Build(ConfigKey, enveloped);
            
            var reader = provider.GetRequiredService<IDisasterCardReader>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<DisasterCardValidationException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Contains("A disaster card with the Name 'Asteroid Impact' already exists. (ID: 2)", exception.Message);
        }

        [Fact]
        public async Task GetAllAsync_WithDataWithoutEnvelope_ThrowsCatalogDataAccessException()
        {
            // Arrange
            var bareFilePath = DisasterCardTestFileCatalog.DataOnly("disaster-cards-test.json");    // Don't add the envelope

            using var provider = _fixture.Build(ConfigKey, bareFilePath);
            
            var reader = provider.GetRequiredService<IDisasterCardReader>();
            
            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Equal(CatalogDataAccessErrorCode.BadJson, exception.ErrorCode);
        }

        public static TheoryData<string, CatalogDataAccessErrorCode> InvalidFileCases()
        {
            return new TheoryData<string, CatalogDataAccessErrorCode>
            {
                { "invalid-json.json", CatalogDataAccessErrorCode.BadJson },
                { "empty.json", CatalogDataAccessErrorCode.DataMissing } 
            };
        }

        private static string EnvelopArray(string filepath)
        {
            var bareFilePath = DisasterCardTestFileCatalog.DataOnly(filepath); 
            return TestJsonEnvelopeCreator.EnvelopArrayFile(bareFilePath);
        }
    }
}
