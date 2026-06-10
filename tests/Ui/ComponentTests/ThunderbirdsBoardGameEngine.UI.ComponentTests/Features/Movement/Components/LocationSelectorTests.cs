using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Movement.Components
{
    public class LocationSelectorTests : BunitContext
    {
        [Fact]
        public void LocationSelectorShouldRenderLocationsCorrectly()
        {
            // Arrange
            var locations = CreateSampleLocations();

            // Act
            var cut = Render<LocationSelector>(parameters => parameters
                .Add(p => p.Locations, locations)
                .Add(p => p.SelectedLocationKey, "earth")
            );

            // Assert
            Assert.Contains("Earth", cut.Markup);
            Assert.Contains("Mars", cut.Markup);
            Assert.Contains("Venus", cut.Markup);
        }

        [Fact]
        public void LocationSelectorShouldNotifyParentOnChange()
        {
            // Arrange
            var locations = CreateSampleLocations();

            string? selectedKey = "mars";

            // Act
            var cut = Render<LocationSelector>(parameters => parameters
                .Add(p => p.Locations, locations)
                .Add(p => p.SelectedLocationKeyChanged, value => selectedKey = value)
            );

            cut.Find("select").Change("venus");

            // Assert
            Assert.Equal("venus", selectedKey);
        }

        private static IReadOnlyList<MovementLocationOptions> CreateSampleLocations()
        {
            return
            [
                new MovementLocationOptions(Key: "earth", DisplayName: "Earth"),
                new MovementLocationOptions(Key: "mars", DisplayName: "Mars"),
                new MovementLocationOptions(Key: "venus", DisplayName: "Venus"),
            ];
        }
    }
}
