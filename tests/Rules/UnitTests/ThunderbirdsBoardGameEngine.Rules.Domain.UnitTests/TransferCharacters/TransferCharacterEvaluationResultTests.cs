using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.TransferCharacters
{
    public class TransferCharacterEvaluationResultTests
    {
        [Fact]
        public void Valid_ShouldReturnValidOutcome()
        {
            // Arrange

            // Act
            var result = TransferCharacterEvaluationResult.Valid();

            // Assert
            Assert.Equal(TransferCharacterOutcome.Valid, result.Outcome);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void NoChange_ShouldReturnNoChangeOutcome()
        {
            // Arrange

            // Act
            var result = TransferCharacterEvaluationResult.NoChange();

            // Assert
            Assert.Equal(TransferCharacterOutcome.NoChange, result.Outcome);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void Rejected_ShouldReturnRejectedOutcomeWithReason()
        {
            // Arrange

            // Act
            var result = TransferCharacterEvaluationResult.Rejected(TransferCharacterRejectionReason.NotReciprocal);

            // Assert
            Assert.Equal(TransferCharacterOutcome.Rejected, result.Outcome);
            Assert.Equal(TransferCharacterRejectionReason.NotReciprocal, result.RejectionReason);
        }
    }
}
