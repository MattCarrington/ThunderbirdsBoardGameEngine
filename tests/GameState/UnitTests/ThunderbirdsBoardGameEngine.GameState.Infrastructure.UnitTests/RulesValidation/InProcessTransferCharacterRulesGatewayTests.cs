using MediatR;
using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure.RulesValidation;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation;
using Xunit;

using GameStateThunderbirdState =
    ThunderbirdsBoardGameEngine.GameState.Application
        .TransferCharacters.CurrentThunderbirdState;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.UnitTests.RulesValidation
{
    public class InProcessTransferCharacterRulesGatewayTests
    {
        [Fact]
        public async Task ValidateTransferCharactersAsync_ShouldReturnPermitted_WhenRulesAllowTransferAsync()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<ValidateCharacterTransferQuery>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ValidateCharacterTransferResponse(true, null)));

            var gateway = new InProcessTransferCharacterRulesGateway(mediator);



            // Act
            var decision = await gateway.ValidateTransferCharactersAsync(
                CreateCurrentState(),
                CreateAssignments(),
                TestContext.Current.CancellationToken);

            // Assert
            Assert.True(decision.IsPermitted);
        }

        [Theory]
        [InlineData(TransferCharacterValidationRejectionReason.NotReciprocal, CharacterTransferRejectionReason.NotReciprocal)]
        [InlineData(TransferCharacterValidationRejectionReason.MachinesNotCoLocated, CharacterTransferRejectionReason.MachinesNotCoLocated)]
        [InlineData(TransferCharacterValidationRejectionReason.CharacterCapacityExceeded, CharacterTransferRejectionReason.CharacterCapacityExceeded)]
        public async Task ValidateAsync_WhenRulesRejects_ShouldMapReason(TransferCharacterValidationRejectionReason rulesReason, CharacterTransferRejectionReason expectedReason)
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();

            mediator.Send(Arg.Any<ValidateCharacterTransferQuery>(), Arg.Any<CancellationToken>())
                .Returns(new ValidateCharacterTransferResponse(
                    IsPermitted: false,
                    RejectionReason: rulesReason));

            var gateway = new InProcessTransferCharacterRulesGateway(mediator);

            // Act
            var result = await gateway.ValidateTransferCharactersAsync(
                CreateCurrentState(),
                CreateAssignments(),
                TestContext.Current.CancellationToken);

            // Assert
            Assert.False(result.IsPermitted);
            Assert.Equal(expectedReason, result.RejectionReason);
        }

        [Fact]
        public async Task ValidateAsync_WhenRulesRejectsWithoutReason_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<ValidateCharacterTransferQuery>(), Arg.Any<CancellationToken>())
                .Returns(new ValidateCharacterTransferResponse(
                    IsPermitted: false,
                    RejectionReason: null));

            var gateway = new InProcessTransferCharacterRulesGateway(mediator);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => gateway.ValidateTransferCharactersAsync(
                CreateCurrentState(),
                CreateAssignments(),
                TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task ValidateAsync_WhenRulesReturnsUnexpectedReason_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<ValidateCharacterTransferQuery>(), Arg.Any<CancellationToken>())
                .Returns(new ValidateCharacterTransferResponse(
                    IsPermitted: false,
                    RejectionReason: (TransferCharacterValidationRejectionReason)999));

            var gateway = new InProcessTransferCharacterRulesGateway(mediator);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => gateway.ValidateTransferCharactersAsync(
                CreateCurrentState(),
                CreateAssignments(),
                TestContext.Current.CancellationToken));
        }

        private static List<GameStateThunderbirdState> CreateCurrentState()
        {
            return
            [
                new(new("TB1"), new("LOC1"), [new("CHAR1")]),
                new(new("TB2"), new("LOC2"), [new("CHAR2")])
            ];
        }

        private static List<CharacterAssignment> CreateAssignments()
        {
            return
            [
                new(new("CHAR1"), new("TB2")),
                new(new("CHAR2"), new("TB1"))
            ];
        }
    }
}
