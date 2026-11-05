using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Application.UnitTests.Exceptions
{
    public class CharacterTransferRejectedExceptionTests
    {
        [Fact]
        public void FromDecision_ShouldThrowArgumentException_WhenDecisionIsPermitted()
        {
            // Arrange
            var permittedDecision = CharacterTransferDecision.Permitted();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CharacterTransferRejectedException.FromDecision(permittedDecision));
        }

        [Theory]
        [InlineData(CharacterTransferRejectionReason.CharacterCapacityExceeded)]
        [InlineData(CharacterTransferRejectionReason.MachinesNotCoLocated)]
        [InlineData(CharacterTransferRejectionReason.NotReciprocal)]
        public void FromDecision_ShouldReturnExceptionWithCorrectReason_WhenDecisionIsRejectedWithReason(CharacterTransferRejectionReason rejectionReason)
        {
            // Arrange
            var rejectedDecisionWithReason = CharacterTransferDecision.Rejected(rejectionReason);

            // Act
            var exception = CharacterTransferRejectedException.FromDecision(rejectedDecisionWithReason);

            // Assert
            Assert.Equal(rejectionReason, exception.Reason);
        }
    }
}
