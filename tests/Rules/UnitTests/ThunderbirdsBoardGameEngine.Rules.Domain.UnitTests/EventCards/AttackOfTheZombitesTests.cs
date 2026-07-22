using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.EventCards
{
    public class AttackOfTheZombitesTests
    {
        [Fact]
        public void ApplyMovementModifier_WhenThunderbird1_ShouldReturnModifier()
        {
            // Arrange
            var attackOfTheZombites = new AttackOfTheZombites();

            // Act
            var result = attackOfTheZombites.ApplyMovementModifier(KnownThunderbirdCodes.Thunderbird1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(KnownEventCardCodes.AttackOfTheZombites, result.Card);
            Assert.Equal(1, result.EffectiveTopSpeed);
            Assert.Equal("Attack of the Zombites: Thunderbird 1's top speed is reduced to 1.", result.Message);
        }

        [Theory]
        [InlineData("thunderbird-2")]
        [InlineData("thunderbird-3")]
        [InlineData("thunderbird-4")]
        [InlineData("thunderbird-5")]
        [InlineData("fab-1")]
        public void ApplyMovementModifier_WhenNotThunderbird1_ShouldReturnNull(string thunderbirdCode)
        {
            // Arrange
            var attackOfTheZombites = new AttackOfTheZombites();

            // Act
            var result = attackOfTheZombites.ApplyMovementModifier(new ThunderbirdCode(thunderbirdCode));

            // Assert
            Assert.Null(result);
        }
    }
}
