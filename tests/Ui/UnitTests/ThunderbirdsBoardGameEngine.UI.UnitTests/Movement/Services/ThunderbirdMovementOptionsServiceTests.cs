using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Movement.Services
{
    public class ThunderbirdMovementOptionsServiceTests
    {
        [Fact]
        public void GetAllMobileVehicles_WhenCalled_ReturnsMobileVehicles()
        {
            // Arrange
            var thunderbird1 = new ReferenceThunderbirdDefinition(
                code: new ThunderbirdCode("TB001"),
                displayName: "Thunderbird 1",
                domain: MovementDomain.Earth,
                topSpeed: 100);

            var thunderbird2 = new ReferenceThunderbirdDefinition(
                code: new ThunderbirdCode("TB002"),
                displayName: "Thunderbird 2",
                domain: MovementDomain.Space,
                topSpeed: 0);

            var thunderbird3 = new ReferenceThunderbirdDefinition(
                code: new ThunderbirdCode("TB003"),
                displayName: "Thunderbird 3",
                domain: MovementDomain.Space,
                topSpeed: 150);

            var catalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            catalog.GetAll().Returns(new[] { thunderbird1, thunderbird2, thunderbird3 }.ToImmutableArray());

            var service = new ThunderbirdMovementOptionsService(catalog);

            // Act            
            var result = service.GetAllMobileVehicles();

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Key == thunderbird1.Code.Value && t.DisplayName == thunderbird1.DisplayName);
            Assert.Contains(result, t => t.Key == thunderbird3.Code.Value && t.DisplayName == thunderbird3.DisplayName);
        }
    }
}
