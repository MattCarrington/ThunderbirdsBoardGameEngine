using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.FabCards
{
    public class RemoteControlHoverCameraTests
    {
        [Fact]
        public void GetBonusModifier_InputIsAirRescue_ReturnsAppliedModifier()
        {
            // Arrange
            var remoteControlHoverCamera = new RemoteControlHoverCamera();

            var input = new RescueCalculationInput([], RescueType.Air);

            // Act
            var result = remoteControlHoverCamera.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single(result);
            Assert.Equal("remote-control-hover-camera", bonus.Key);
            Assert.Equal(3, bonus.Value);
            Assert.Equal(SourceType.FabCard, bonus.SourceType);
        }

        [Theory]
        [InlineData(RescueType.Land)]
        [InlineData(RescueType.Sea)]
        [InlineData(RescueType.Space)]
        public void GetBonusModifier_InputIsNotAirRescue_ReturnsNoAppliedModifier(RescueType rescueType)
        {
            // Arrange
            var remoteControlHoverCamera = new RemoteControlHoverCamera();

            var input = new RescueCalculationInput(Array.Empty<DisasterBonusKey>(), rescueType);

            // Act
            var result = remoteControlHoverCamera.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }
    }
}
