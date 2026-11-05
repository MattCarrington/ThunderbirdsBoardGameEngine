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
            var result = TransferCharacterEvaluationResult.Permitted();

            // Assert
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void Rejected_ShouldReturnRejectedOutcomeWithReason()
        {
            // Arrange

            // Act
            var result = TransferCharacterEvaluationResult.Rejected(TransferCharacterRejectionReason.NotReciprocal);

            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterRejectionReason.NotReciprocal, result.RejectionReason);
        }
    }
}
