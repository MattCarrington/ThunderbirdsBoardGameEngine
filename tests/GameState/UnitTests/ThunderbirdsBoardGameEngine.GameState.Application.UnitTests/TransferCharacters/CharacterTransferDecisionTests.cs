using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.TransferCharacters
{
    public class CharacterTransferDecisionTests
    {
        [Fact]
        public void Permitted_ShouldCreatePermittedDecision_WhenCalled()
        {
            // Act
            var decision = CharacterTransferDecision.Permitted();
            // Assert
            Assert.True(decision.IsPermitted);
            Assert.Null(decision.RejectionReason);
        }

        [Theory]
        [InlineData(CharacterTransferRejectionReason.CharacterCapacityExceeded)]
        [InlineData(CharacterTransferRejectionReason.MachinesNotCoLocated)]
        [InlineData(CharacterTransferRejectionReason.NotReciprocal)]
        public void Rejected_ShouldCreateRejectedDecision_WhenCalled(CharacterTransferRejectionReason reason)
        {
            // Arrange

            // Act
            var decision = CharacterTransferDecision.Rejected(reason);

            // Assert
            Assert.False(decision.IsPermitted);
            Assert.Equal(reason, decision.RejectionReason);
        }
    }
}
