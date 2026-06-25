using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
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

            var result = catalog.GetAll();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(6, result.Length);
        }

        [Fact]
        public void CanLoadKnownCharacterDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<ICharacterDefinitionCatalog>();

            // Act            
            var result = catalog.GetByCode(new CharacterCode("gordon"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("gordon", result.Code.ToString());
            Assert.Equal("Gordon", result.DisplayName);
        }
    }
}