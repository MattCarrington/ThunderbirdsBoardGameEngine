using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Shared.Components
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

        private static IReadOnlyList<LocationOptionsViewModel> CreateSampleLocations()
        {
            return
            [
                new LocationOptionsViewModel(Key: "earth", DisplayName: "Earth"),
                new LocationOptionsViewModel(Key: "mars", DisplayName: "Mars"),
                new LocationOptionsViewModel(Key: "venus", DisplayName: "Venus"),
            ];
        }
    }
}
