using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Services
{
    public class CharactersServiceTests
    {
        [Fact]
        public void GetAll_WhenCalled_CallsCatalog()
        {
            // Arrange
            var catalog = Substitute.For<ICharacterDefinitionCatalog>();
            catalog.GetAll().Returns(ImmutableArray<ReferenceCharacterDefinition>.Empty);

            var service = new CharacterService(catalog);

            // Act
            service.GetAll();

            // Assert
            catalog.Received(1).GetAll();
        }

        [Fact]
        public void GetAll_WhenCatalogReturnsMultipleItems_ReturnsCorrectCount()
        {
            // Arrange
            var character1 = new ReferenceCharacterDefinition(
                code: new CharacterCode("CH001"),
                displayName: "Character 1",
                rescueBonus: null
            );
            var character2 = new ReferenceCharacterDefinition(
                code: new CharacterCode("CH002"),
                displayName: "Character 2",
                rescueBonus: null
            );
            var character3 = new ReferenceCharacterDefinition(
                code: new CharacterCode("CH003"),
                displayName: "Character 3",
                rescueBonus: null
            );

            var catalog = Substitute.For<ICharacterDefinitionCatalog>();
            catalog.GetAll().Returns(new[] { character1, character2, character3 }.ToImmutableArray());

            var service = new CharacterService(catalog);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetAll_WhenCatalogIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var catalog = Substitute.For<ICharacterDefinitionCatalog>();
            catalog.GetAll().Returns(ImmutableArray<ReferenceCharacterDefinition>.Empty);

            var service = new CharacterService(catalog);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
