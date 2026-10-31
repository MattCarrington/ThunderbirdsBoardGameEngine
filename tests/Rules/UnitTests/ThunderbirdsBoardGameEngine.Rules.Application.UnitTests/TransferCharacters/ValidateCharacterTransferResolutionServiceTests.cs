using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.TransferCharacters
{
    public class ValidateCharacterTransferResolutionServiceTests
    {
        [Fact]
        public void ResolveTransferValidation_WhenCapacitiesAreResolved_ReturnsEvaluatorResult()
        {
            // Arrange
            var input = CreateValidInput();

            var thunderbird1 = input.Thunderbirds.First();
            var thunderbird2 = input.Thunderbirds.Skip(1).First();

            var lookup = Substitute.For<IThunderbirdTransferDefinitionLookup>();
            lookup.GetThunderbirdCharacterCapacity(Arg.Is(thunderbird1.ThunderbirdCode))
                .Returns(1);
            lookup.GetThunderbirdCharacterCapacity(Arg.Is(thunderbird2.ThunderbirdCode))
                .Returns(2);

            var evaluator = new TransferCharacterEvaluator();

            var service = new ValidateCharacterTransferResolutionService(lookup, evaluator);

            // Act
            var result = service.ResolveTransferValidation(input);

            // Assert
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);

            lookup.Received(1).GetThunderbirdCharacterCapacity(thunderbird1.ThunderbirdCode);
            lookup.Received(1).GetThunderbirdCharacterCapacity(thunderbird2.ThunderbirdCode);
        }

        [Fact]
        public void ResolveTransferValidation_WhenDefinitionIsMissing_ShouldThrow()
        {
            var missingCode = new ThunderbirdCode("TB-2");

            var lookup = Substitute.For<IThunderbirdTransferDefinitionLookup>();

            lookup.GetThunderbirdCharacterCapacity(missingCode)
                .Throws(new ReferenceDataNotFoundException(
                    "Thunderbird",
                    missingCode.Value));

            var service = new ValidateCharacterTransferResolutionService(
                lookup,
                new TransferCharacterEvaluator());

            var input = CreateValidInput();

            Assert.Throws<ReferenceDataNotFoundException>(() => service.ResolveTransferValidation(input));
        }

        [Fact]
        public void ResolveTransferValidation_WhenInputIsNull_ShouldThrow()
        {
            // Arrange
            var lookup = Substitute.For<IThunderbirdTransferDefinitionLookup>();

            var evaluator = new TransferCharacterEvaluator();

            var service = new ValidateCharacterTransferResolutionService(lookup, evaluator);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.ResolveTransferValidation(null));
        }

        private static ValidateCharacterTransferInput CreateValidInput()
        {
            var thunderbird1 = new CurrentThunderbirdState(
                ThunderbirdCode: new ThunderbirdCode("TB-1"),
                CurrentLocation: new LocationCode("Location-1"),
                Occupants: [new CharacterCode("Char-1")]);

            var thunderbird2 = new CurrentThunderbirdState(
                ThunderbirdCode: new ThunderbirdCode("TB-2"),
                CurrentLocation: new LocationCode("Location-1"),
                Occupants: [new CharacterCode("Char-2"), new CharacterCode("Char-3")]);

            var assignments = new List<RequestedCharacterAssignment>
            {
                new(new CharacterCode("Char-1"), new ThunderbirdCode("TB-2")),
                new(new CharacterCode("Char-2"), new ThunderbirdCode("TB-1"))
            };

            return new ValidateCharacterTransferInput(
                Thunderbirds: [thunderbird1, thunderbird2],
                Assignments: assignments);
        }
    }
}
