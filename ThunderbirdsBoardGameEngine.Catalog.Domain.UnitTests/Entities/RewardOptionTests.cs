using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Entities
{
    public class RewardOptionTests
    {
        [Fact]
        public void SpecifiedToken_WhenCalledWithValidToken_CreatesInstance()
        {
            // Arrange
            var token = Enums.BonusToken.Intelligence;

            // Act
            var rewardOption = RewardOption.SpecifiedToken(token);

            // Assert
            Assert.False(rewardOption.IsUserChoice);
            Assert.Equal(token, rewardOption.Token);
        }

        [Fact]
        public void PlayerChoice_WhenCalled_CreatesInstance()
        {
            // Act
            var rewardOption = RewardOption.PlayerChoice();

            // Assert
            Assert.True(rewardOption.IsUserChoice);
            Assert.Null(rewardOption.Token);
        }
    }
}
