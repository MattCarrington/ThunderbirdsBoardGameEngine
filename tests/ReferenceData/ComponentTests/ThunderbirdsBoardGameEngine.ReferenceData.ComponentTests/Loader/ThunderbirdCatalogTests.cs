using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Loader
{
    public class ThunderbirdCatalogTests
    {
        [Fact]
        public void CanLoadKnownThunderbirdDefinition()
        {
            // Arrange
            using var provider = ReferenceDataTestHost.BuildServiceProvider();

            var catalog = provider.GetRequiredService<IThunderbirdDefinitionCatalog>();

            // Act            
            var result = catalog.GetByCode(new ThunderbirdCode("thunderbird-1"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("thunderbird-1", result.Code.ToString());
            Assert.Equal("Thunderbird 1", result.DisplayName);
            Assert.Equal(MovementDomain.Earth, result.Domain);
            Assert.Equal(3, result.TopSpeed);
        }
    }
}
