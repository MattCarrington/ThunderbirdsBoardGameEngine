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
            var evaluationResult = TransferCharacterEvaluationResult.Valid();

            // Act
            var result = evaluationResult.ToResponse();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.Valid, result.TransferCharacterValidationOutcome);
            Assert.Null(result.TransferCharacterValidationRejectionReason);
        }

        [Fact]
        public void ToResponse_NoChangeOutcome_ReturnsNoChangeResponse()
        {
            // Arrange
            var evaluationResult = TransferCharacterEvaluationResult.NoChange();

            // Act
            var result = evaluationResult.ToResponse();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.NoChange, result.TransferCharacterValidationOutcome);
            Assert.Null(result.TransferCharacterValidationRejectionReason);
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
            Assert.Equal(TransferCharacterValidationState.Rejected, result.TransferCharacterValidationOutcome);
            Assert.Equal(TransferCharacterValidationRejectionReason.NotReciprocal, result.TransferCharacterValidationRejectionReason);
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
