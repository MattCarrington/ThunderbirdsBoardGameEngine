using ThunderbirdsBoardGameEngine.Api.Presentation;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Presentation
{
    public class EnumDisplayHelperTests
    {
        [Theory]
        [InlineData(ThunderbirdMachine.Thunderbird1, "Thunderbird 1")]
        [InlineData(PodVehicle.Domo, "DOMO")]
        [InlineData(BoardLocation.GeoStationaryOrbit, "Geo-Stationary Orbit")]
        [InlineData(Character.Gordon, "Gordon")]
        [InlineData(RescueType.Air, "Air")]
        public void GetDisplayName_ReturnsExpectedDisplayValue<TEnum>(TEnum value, string expected)
            where TEnum : struct, Enum
        {
            // Arrange

            // Act
            var result = EnumDisplayHelper.GetDisplayName(value);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
