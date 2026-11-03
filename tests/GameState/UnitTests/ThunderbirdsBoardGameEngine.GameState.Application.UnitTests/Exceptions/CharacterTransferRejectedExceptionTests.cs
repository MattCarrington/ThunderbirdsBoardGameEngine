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
            var exception = Assert.Throws<ArgumentException>(() =>
                CharacterTransferRejectedException.FromDecision(permittedDecision));

            Assert.Contains("A permitted decision cannot produce a rejection exception.", exception.Message);
            Assert.Equal("decision", exception.ParamName);
        }

        [Fact]
        public void FromDecision_ShouldThrowInvalidOperationException_WhenDecisionIsRejectedWithoutReason()
        {
            // Arrange
            var rejectedDecisionWithoutReason = new CharacterTransferDecision(false, null);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                CharacterTransferRejectedException.FromDecision(rejectedDecisionWithoutReason));

            Assert.Contains("A rejected transfer decision must provide a reason.", exception.Message);
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
            var exception = Assert.Throws<InvalidOperationException>(() =>
                CharacterTransferRejectedException.FromDecision(decision));

            Assert.Contains("A no-change rejection cannot originate from the Rules gateway.", exception.Message);
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
