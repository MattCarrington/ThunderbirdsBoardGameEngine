using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Movement.Components
{
    public class ThunderbirdSelectorTests : BunitContext
    {
        [Fact]
        public void ThunderbirdSelectorShouldRenderThunderbirdsCorrectly()
        {
            // Arrange
            var thunderbirds = CreateSampleThunderbirds();

            // Act
            var cut = Render<ThunderbirdSelector>(parameters => parameters
                .Add(p => p.Thunderbirds, thunderbirds)
                .Add(p => p.SelectedThunderbirdCode, "tb1")
            );

            // Assert
            Assert.Contains("Thunderbird 1", cut.Markup);
            Assert.Contains("Thunderbird 2", cut.Markup);
            Assert.Contains("Thunderbird 3", cut.Markup);
        }

        [Fact]
        public void ThunderbirdSelectorShouldNotifyParentOnChange()
        {
            // Arrange
            var thunderbirds = CreateSampleThunderbirds();

            string? selectedKey = "tb3";

            // Act
            var cut = Render<ThunderbirdSelector>(parameters => parameters
                .Add(p => p.Thunderbirds, thunderbirds)
                .Add(p => p.SelectedThunderbirdCodeChanged, value => selectedKey = value)
            );

            cut.Find("select").Change("tb2");

            // Assert
            Assert.Equal("tb2", selectedKey);
        }

        private static IReadOnlyList<ThunderbirdMovementOptions> CreateSampleThunderbirds()
        {
            return
            [
                new ThunderbirdMovementOptions(Key: "tb1", DisplayName: "Thunderbird 1"),
                new ThunderbirdMovementOptions(Key: "tb2", DisplayName: "Thunderbird 2"),
                new ThunderbirdMovementOptions(Key: "tb3", DisplayName: "Thunderbird 3"),
            ];
        }
    }
}
