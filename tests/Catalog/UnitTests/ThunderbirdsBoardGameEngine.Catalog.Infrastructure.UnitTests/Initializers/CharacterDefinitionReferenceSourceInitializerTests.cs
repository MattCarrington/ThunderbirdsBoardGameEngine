using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initializers;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Initializers
{
    public class CharacterDefinitionReferenceSourceInitializerTests
    {
        [Fact]
        public async Task InitializeAsync_WithValidCharacters_ReturnsReferenceSourceAsync()
        {
            // Arrange
            var characters = TestCharacters.ValidSix;

            var reader = Substitute.For<ICharacterReader>();
            reader.GetAllAsync(Arg.Any<CancellationToken>()).Returns(characters);

            var initializer = CreateInitializer(reader);

            // Act
            var result = await initializer.InitializeAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Characters.Length);
        }

        [Fact]
        public async Task InitializeAsync_WithNoCharacters_ThrowsInvalidDataException()
        {
            // Arrange
            var characters = Array.Empty<CharacterDefinition>();

            var reader = Substitute.For<ICharacterReader>();
            reader.GetAllAsync(Arg.Any<CancellationToken>()).Returns(characters);

            var initializer = CreateInitializer(reader);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => initializer.InitializeAsync(CancellationToken.None));
        }

        [Fact]
        public async Task InitializeAsync_WithNullCharacters_ThrowsArgumentNullException()
        {
            // Arrange
            var reader = Substitute.For<ICharacterReader>();
            reader.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<CharacterDefinition>>(null));

            var initializer = CreateInitializer(reader);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => initializer.InitializeAsync(CancellationToken.None));
        }

        private static CharacterDefinitionReferenceSourceInitializer CreateInitializer(ICharacterReader reader)
        {
            return new CharacterDefinitionReferenceSourceInitializer(reader);
        }
    }
}
