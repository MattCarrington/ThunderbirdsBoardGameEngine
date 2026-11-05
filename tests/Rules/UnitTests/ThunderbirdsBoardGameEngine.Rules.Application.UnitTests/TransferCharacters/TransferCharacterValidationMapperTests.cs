using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.TransferCharacters
{
    public class TransferCharacterValidationMapperTests
    {
        [Fact]
        public void ToResponse_ValidOutcome_ReturnsValidResponse()
        {
            // Arrange
            var evaluationResult = TransferCharacterEvaluationResult.Permitted();

            // Act
            var result = evaluationResult.ToResponse();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public void ToResponse_RejectedOutcomeWithReason_ReturnsRejectedResponse()
        {
            // Arrange
            var evaluationResult = TransferCharacterEvaluationResult.Rejected(TransferCharacterRejectionReason.NotReciprocal);

            // Act
            var result = evaluationResult.ToResponse();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterValidationRejectionReason.NotReciprocal, result.RejectionReason);
        }

        [Fact]
        public void MapRejectionReason_UnknownReason_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var evaluationResult = TransferCharacterEvaluationResult.Rejected((TransferCharacterRejectionReason)999); // Invalid reason

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => evaluationResult.ToResponse());
        }
    }
}
