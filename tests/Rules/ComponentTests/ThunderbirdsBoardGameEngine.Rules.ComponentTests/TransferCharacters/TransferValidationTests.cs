using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
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
                CurrentState: [
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
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public async Task TransferRejectedDueToFullCapacity()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                CurrentState: [
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
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterValidationRejectionReason.CharacterCapacityExceeded, result.RejectionReason);
        }

        [Fact]
        public async Task TransferRejectedDueToLocationMismatch()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                CurrentState: [
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
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterValidationRejectionReason.MachinesNotCoLocated, result.RejectionReason);
        }

        [Fact]
        public async Task ValidReciprocalSwap()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                CurrentState: [
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
            Assert.True(result.IsPermitted);
            Assert.Null(result.RejectionReason);
        }

        [Fact]
        public async Task TransferRejectedDueToNonReciprocalTransfer()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                CurrentState: [
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
            Assert.False(result.IsPermitted);
            Assert.Equal(TransferCharacterValidationRejectionReason.NotReciprocal, result.RejectionReason);
        }

        [Fact]
        public async Task ThrowsWhenNonExistantThunderbird()
        {
            // Arrange
            var mediator = CreateMediator();

            var request = new ValidateCharacterTransferQuery(
                CurrentState: [
                    new(KnownThunderbirdCodes.Thunderbird1, KnownLocationCodes.Europe, [ KnownCharacterCodes.Scott ])
                    ],
                ProposedAssignments: [
                    new(KnownCharacterCodes.Scott, new ThunderbirdCode("thunderbird-999"))
                    ]);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => mediator.Send(request, TestContext.Current.CancellationToken));
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
