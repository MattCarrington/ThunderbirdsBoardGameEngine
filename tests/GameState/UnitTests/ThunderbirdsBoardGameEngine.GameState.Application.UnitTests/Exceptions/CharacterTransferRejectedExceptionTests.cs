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
            var permittedDecision = new CharacterTransferDecision(true, null);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CharacterTransferRejectedException.FromDecision(permittedDecision));
        }

        [Fact]
        public void FromDecision_ShouldThrowInvalidOperationException_WhenDecisionIsRejectedWithoutReason()
        {
            // Arrange
            var rejectedDecisionWithoutReason = new CharacterTransferDecision(false, null);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => CharacterTransferRejectedException.FromDecision(rejectedDecisionWithoutReason));
        }

        [Theory]
        [InlineData(CharacterTransferRejectionReason.CharacterCapacityExceeded)]
        [InlineData(CharacterTransferRejectionReason.MachinesNotCoLocated)]
        [InlineData(CharacterTransferRejectionReason.NotReciprocal)]
        public void FromDecision_ShouldReturnExceptionWithCorrectReason_WhenDecisionIsRejectedWithReason(CharacterTransferRejectionReason rejectionReason)
        {
            // Arrange
            var rejectedDecisionWithReason = new CharacterTransferDecision(false, rejectionReason);

            // Act
            var exception = CharacterTransferRejectedException.FromDecision(rejectedDecisionWithReason);

            // Assert
            Assert.Equal(rejectionReason, exception.Reason);
        }

        [Fact]
        public void FromDecision_ShouldThrowInvalidOperationException_WhenReasonIsNoChange()
        {
            // Arrange
            var decision = new CharacterTransferDecision(false, CharacterTransferRejectionReason.NoChange);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => CharacterTransferRejectedException.FromDecision(decision));
        }

        [Fact]
        public void NoChange_ShouldReturnExceptionWithNoChangeReason_WhenCalled()
        {
            // Arrange

            // Act
            var exception = CharacterTransferRejectedException.NoChange();

            // Assert
            Assert.Equal(CharacterTransferRejectionReason.NoChange, exception.Reason);
        }
    }
}
