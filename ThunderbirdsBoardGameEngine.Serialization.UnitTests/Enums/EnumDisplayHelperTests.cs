using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.Serialization.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Serialization.UnitTests.Enums
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
            var result = EnumDisplayHelper.GetDisplayName(value);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Thunderbird 1", ThunderbirdMachine.Thunderbird1)]
        [InlineData("lady penelope", Character.LadyPenelope)]
        [InlineData("gordon", Character.Gordon)]
        [InlineData("  DOMO  ", PodVehicle.Domo)]
        [InlineData("excavator", PodVehicle.Excavator)]
        [InlineData("North Pacific", BoardLocation.NorthPacific)]
        [InlineData("north-pacific", BoardLocation.NorthPacific)]
        [InlineData("NORTH PACIFIC", BoardLocation.NorthPacific)]
        public void ParseFromDisplayName_ReturnsExpectedEnum<TEnum>(string input, TEnum expected)
            where TEnum : struct, Enum
        {
            var result = EnumDisplayHelper.ParseFromDisplayName<TEnum>(input);
            Assert.Equal(expected, result);
        }
    }
}
