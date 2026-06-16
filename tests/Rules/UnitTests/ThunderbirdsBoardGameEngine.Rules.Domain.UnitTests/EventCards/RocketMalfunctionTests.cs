using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.EventCards
{
    public class RocketMalfunctionTests
    {
        [Fact]
        public void ApplyMovementModifier_WhenThunderbird3_ShouldReturnModifier()
        {
            // Arrange
            var rocketMalfunction = new RocketMalfunction();

            // Act
            var result = rocketMalfunction.ApplyMovementModifier(KnownThunderbirdCodes.Thunderbird3);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(KnownEventCardCodes.RocketMalfunction, result.Card);
            Assert.Equal(1, result.TopSpeedModifier);
            Assert.Equal("Rocket Malfunction: Thunderbird 3's top speed is reduced to 1.", result.Message);
        }

        [Theory]
        [InlineData("thunderbird-1")]
        [InlineData("thunderbird-2")]
        [InlineData("thunderbird-4")]
        [InlineData("thunderbird-5")]
        [InlineData("fab-1")]
        public void ApplyMovementModifier_WhenNotThunderbird3_ShouldReturnNull(string thunderbirdCode)
        {
            // Arrange
            var rocketMalfunction = new RocketMalfunction();

            // Act
            var result = rocketMalfunction.ApplyMovementModifier(new ThunderbirdCode(thunderbirdCode));

            // Assert
            Assert.Null(result);
        }
    }
}
