using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.TransferCharacters
{
    public class TransferValidationTests
    {
        [Fact]
        public async Task ValidSingleTransferAsync()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                GameState: [
                    new(KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.Europe, [ KnownCharacterCodes.Scott ]),
                    new(KnownThunderbirdCodes.Thunderbird2, KnownLocationCodes.Europe, [ KnownCharacterCodes.Virgil ])
                    ],
                ProposedAssignments: [
                    new(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird2)
                    ]);

            // Act
            var result = await mediator.Send(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.Valid, result.TransferCharacterValidationOutcome);
            Assert.Null(result.TransferCharacterValidationRejectionReason);
        }

        [Fact]
        public async Task TransferRejectedDueToFullCapacity()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                GameState: [
                    new(KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.Europe, [ KnownCharacterCodes.Scott ]),
                    new(KnownThunderbirdCodes.Thunderbird4, KnownLocationCodes.Europe, [ KnownCharacterCodes.Gordon ])
                    ],
                ProposedAssignments: [
                    new(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird4)
                    ]);

            // Act
            var result = await mediator.Send(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.Rejected, result.TransferCharacterValidationOutcome);
            Assert.Equal(TransferCharacterValidationRejectionReason.CharacterCapacityExceeded, result.TransferCharacterValidationRejectionReason);
        }

        [Fact]
        public async Task TransferRejectedDueToLocationMismatch()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                GameState: [
                    new(KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.Europe, [ KnownCharacterCodes.Scott ]),
                    new(KnownThunderbirdCodes.Thunderbird2, KnownLocationCodes.SouthPacific, [ KnownCharacterCodes.Virgil ])
                    ],
                ProposedAssignments: [
                    new(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird2)
                    ]);

            // Act
            var result = await mediator.Send(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.Rejected, result.TransferCharacterValidationOutcome);
            Assert.Equal(TransferCharacterValidationRejectionReason.MachinesNotCoLocated, result.TransferCharacterValidationRejectionReason);
        }

        [Fact]
        public async Task ValidReciprocalSwap()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                GameState: [
                    new(KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.Europe, [ KnownCharacterCodes.Scott ]),
                    new(KnownThunderbirdCodes.Thunderbird4, KnownLocationCodes.Europe, [ KnownCharacterCodes.Gordon ])
                    ],
                ProposedAssignments: [
                    new(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird4),
                    new(KnownCharacterCodes.Gordon, KnownThunderbirdCodes.Thunderbird1)
                    ]);

            // Act
            var result = await mediator.Send(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.Valid, result.TransferCharacterValidationOutcome);
            Assert.Null(result.TransferCharacterValidationRejectionReason);
        }

        [Fact]
        public async Task TransferRejectedDueToNonReciprocalTransfer()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                GameState: [
                    new(KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.Europe, [ KnownCharacterCodes.Scott, KnownCharacterCodes.Gordon ]),
                    new(KnownThunderbirdCodes.Thunderbird2, KnownLocationCodes.Europe, [ KnownCharacterCodes.Virgil ])
                    ],
                ProposedAssignments: [
                    new(KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird2),
                    new(KnownCharacterCodes.Gordon, KnownThunderbirdCodes.Thunderbird2)
                    ]);

            // Act
            var result = await mediator.Send(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TransferCharacterValidationState.Rejected, result.TransferCharacterValidationOutcome);
            Assert.Equal(TransferCharacterValidationRejectionReason.NotReciprocal, result.TransferCharacterValidationRejectionReason);
        }

        private static IMediator CreateMediator()
        {
            var services = new ServiceCollection();

            services.AddRules();
            services.AddFakeCatalogs();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }
    }
}
