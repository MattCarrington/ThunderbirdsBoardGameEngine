using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class CharacterCatalogTests
    {
        [Fact]
        public void CanLoadCharacterDefinitions()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<ICharacterDefinitionCatalog>();
            // Act

            var characterDefinitions = catalog.GetAll();

            // Assert
            Assert.NotEmpty(characterDefinitions);
            Assert.Equal(6, characterDefinitions.Length);
        }

        [Fact]
        public void CanLoadKnownCharacterDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<ICharacterDefinitionCatalog>();

            // Act            
            var characterDefinition = catalog.GetByCode(new CharacterCode("gordon"));

            // Assert
            Assert.NotNull(characterDefinition);
            Assert.Equal("gordon", characterDefinition.Code.ToString());
            Assert.Equal("Gordon", characterDefinition.DisplayName);
        }
    }
}
