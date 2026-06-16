using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.EventCards
{
    public class UsnSentinelMissileStrikeTests
    {
        [Fact]
        public void ApplyMovementModifier_WhenThunderbird2_ShouldReturnModifier()
        {
            // Arrange
            var usnSentinelMissileStrike = new UsnSentinelMissileStrike();

            // Act
            var result = usnSentinelMissileStrike.ApplyMovementModifier(KnownThunderbirdCodes.Thunderbird2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(KnownEventCardCodes.UsnSentinelMissileStrike, result.Card);
            Assert.Equal(1, result.TopSpeedModifier);
            Assert.Equal("USN Sentinel Missile Strike: Thunderbird 2's top speed is reduced to 1.", result.Message);
        }

        [Theory]
        [InlineData("thunderbird-1")]
        [InlineData("thunderbird-3")]
        [InlineData("thunderbird-4")]
        [InlineData("thunderbird-5")]
        [InlineData("fab-1")]
        public void ApplyMovementModifier_WhenNotThunderbird2_ShouldReturnNull(string thunderbirdCode)
        {
            // Arrange
            var usnSentinelMissileStrike = new UsnSentinelMissileStrike();

            // Act
            var result = usnSentinelMissileStrike.ApplyMovementModifier(new ThunderbirdCode(thunderbirdCode));

            // Assert
            Assert.Null(result);
        }
    }
}
