using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.ComponentTests.Readers
{
    public class CharacterDefinitionReaderTests : IClassFixture<InfrastructureRegistrationFixture>
    {
        private readonly InfrastructureRegistrationFixture _fixture;

        private const string ConfigKey = "Catalog:Characters:Json:FilePath";

        public CharacterDefinitionReaderTests(InfrastructureRegistrationFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetAllAsync_WithValidFile_ReturnsCharacters()
        {
            // Arrange
            var filePath = CharacterDefinitionTestFileCatalog.Data("characters.json");

            using var provider = _fixture.Build<CharacterDefinitionJsonOptions>(ConfigKey, filePath);
            using var scope = provider.CreateScope();

            var reader = scope.ServiceProvider.GetRequiredService<ICharacterDefinitionReader>();

            // Act
            var characters = await reader.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(characters);
            Assert.NotEmpty(characters);
            Assert.Equal(6, characters.Count); // match your test data
        }

        [Fact]
        public async Task GetAllAsync_WithInvalidCharacters_ThrowsCharacterDefinitionValidationException()
        {
            // Arrange
            var filePath = CharacterDefinitionTestFileCatalog.Data("invalid-characters.json");

            using var provider = _fixture.Build<CharacterDefinitionJsonOptions>(ConfigKey, filePath);
            using var scope = provider.CreateScope();

            var reader = scope.ServiceProvider.GetRequiredService<ICharacterDefinitionReader>();

            // Act & Assert
            await Assert.ThrowsAsync<CharacterDefinitionValidationException>(() => reader.GetAllAsync(CancellationToken.None));
        }

        [Theory]
        [MemberData(nameof(InvalidFileTestCases.InvalidFileCases),
            MemberType = typeof(InvalidFileTestCases))]
        public async Task GetAllAsync_WithInvalidFiles_ThrowsCatalogDataAccessException(string filename, CatalogDataAccessErrorCode expectedErrorCode)
        {
            // Arrange
            var filePath = SharedTestFileCatalog.Invalid(filename);

            using var provider = _fixture.Build<CharacterDefinitionJsonOptions>(ConfigKey, filePath);
            using var scope = provider.CreateScope();

            var reader = scope.ServiceProvider.GetRequiredService<ICharacterDefinitionReader>();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CatalogDataAccessException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Equal(expectedErrorCode, ex.ErrorCode);
        }

        [Fact]
        public async Task GetAllAsync_WhenFileUnreadable_ThrowsSourceUnreadableException()
        {
            // Arrange
            var path = Path.Combine(Path.GetTempPath(),
                $"unreadable-{Path.GetRandomFileName()}.json");

            await File.WriteAllTextAsync(path, "{ \"x\": 1 }");

            using var _ = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

            using var provider = _fixture.Build<CharacterDefinitionJsonOptions>(ConfigKey, path);
            using var scope = provider.CreateScope();

            var reader = scope.ServiceProvider.GetRequiredService<ICharacterDefinitionReader>();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => reader.GetAllAsync(CancellationToken.None));
            Assert.Equal(CatalogDataAccessErrorCode.SourceUnreadable, exception.ErrorCode);
        }
    }
}
