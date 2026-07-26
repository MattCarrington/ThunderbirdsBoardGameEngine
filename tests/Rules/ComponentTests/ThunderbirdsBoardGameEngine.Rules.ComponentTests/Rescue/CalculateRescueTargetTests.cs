using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Rescue
{
    public class CalculateRescueTargetTests
    {
        [Fact]
        public async Task RescueTargetHandlesNoBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.SunProbe,
                PerformingCharacter: new CharacterCode("scott"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes: [],
                ActiveEventCardCodes: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(11, result.TargetNumber);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        [Fact]
        public async Task RescueTargetAccountsForDisasterBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.SunProbe,
                PerformingCharacter: new CharacterCode("scott"),
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("scott"),
                    new DisasterBonusKey("transmitter-truck")
                ],
                PlayedFabCardCodes: [],
                ActiveEventCardCodes: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result.TargetNumber);
            Assert.Equal(5, result.TotalBonus);
            Assert.Equal(2, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "scott" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "transmitter-truck" && b.Value == 3);
        }

        [Fact]
        public async Task RescueTargetAccountsForCharacterBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.PitOfPeril,
                PerformingCharacter: new CharacterCode("virgil"),
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("gordon")
                ],
                PlayedFabCardCodes: [],
                ActiveEventCardCodes: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(9, result.TargetNumber);
            Assert.Equal(2, result.TotalBonus);

            var modifier = Assert.Single(result.AppliedBonuses);
            Assert.Equal("virgil", modifier.Key);
            Assert.Equal(2, modifier.Value);
        }

        [Fact]
        public async Task RescueTargetAccountsForFabCardBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.TerrorInNewYorkCity,
                PerformingCharacter: new CharacterCode("scott"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes:
                [
                    new CardCode("underwater-sealing-unit")
                ],
                ActiveEventCardCodes: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(8, result.TargetNumber);
            Assert.Equal(3, result.TotalBonus);

            var modifier = Assert.Single(result.AppliedBonuses);
            Assert.Equal("underwater-sealing-unit", modifier.Key);
            Assert.Equal(3, modifier.Value);
            Assert.Equal(SourceType.FabCard, modifier.SourceType);
        }

        [Fact]
        public async Task RescueTargetAccountsForEventCardBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.PitOfPeril,
                PerformingCharacter: new CharacterCode("gordon"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes: [],
                ActiveEventCardCodes:
                [
                    new CardCode("the-hood-interferes")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(13, result.TargetNumber);
            Assert.Equal(-2, result.TotalBonus);

            var modifier = Assert.Single(result.AppliedBonuses);
            Assert.Equal("the-hood-interferes", modifier.Key);
            Assert.Equal(-2, modifier.Value);
        }

        [Fact]
        public async Task RescueTargetAccountsAllPossibleBonuses()

        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.TerrorInNewYorkCity,
                PerformingCharacter: new CharacterCode("gordon"),
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("thunderbird-4"),
                    new DisasterBonusKey("virgil"),
                    new DisasterBonusKey("firefly")
                ],
                PlayedFabCardCodes:
                [
                    new CardCode("underwater-sealing-unit")
                ],
                ActiveEventCardCodes:
                [
                    new CardCode("the-hood-interferes")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(0, result.TargetNumber);
            Assert.Equal(11, result.TotalBonus);
            Assert.Equal(6, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "thunderbird-4" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "virgil" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "firefly" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "gordon" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "underwater-sealing-unit" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "the-hood-interferes" && b.Value == -2);
        }

        [Fact]
        public async Task ThrowsInvalidRescueCalculationRequestExceptionWhenInvalidFabCard()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.TerrorInNewYorkCity,
                PerformingCharacter: new CharacterCode("gordon"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes:
                [
                    new CardCode("invalid-card")
                ],
                ActiveEventCardCodes: []
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidRescueCalculationRequestException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsInvalidRescueCalculationRequestExceptionWhenInvalidEventCard()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.TerrorInNewYorkCity,
                PerformingCharacter: new CharacterCode("gordon"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes: [],
                ActiveEventCardCodes:
                [
                    new CardCode("invalid-card")
                ]
            );

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidRescueCalculationRequestException>(() => mediator.Send(request, CancellationToken.None));
        }

        [Fact]
        public async Task RescueTargetIgnoresValidCardNotInRegistry()
        {
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.TerrorInNewYorkCity,
                PerformingCharacter: new CharacterCode("virgil"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes:
                [
                    new CardCode("jeff-s-orders")
                ],
                ActiveEventCardCodes:
                [
                    new CardCode("explosion-on-tracy-island")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(11, result.TargetNumber);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        [Fact]
        public async Task RescueTargetEnsuresFabCardOnlyCountsOnce()
        {
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.TerrorInNewYorkCity,
                PerformingCharacter: new CharacterCode("virgil"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes:
                [
                    new CardCode("underwater-sealing-unit"),
                    new CardCode("underwater-sealing-unit")
                ],
                ActiveEventCardCodes: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(8, result.TargetNumber);
            Assert.Equal(3, result.TotalBonus);
            Assert.Single(result.AppliedBonuses);
        }

        [Fact]
        public async Task RescueTargetEnsuresEventCardOnlyCountsOnce()
        {
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: TestDisasterCodes.PitOfPeril,
                PerformingCharacter: new CharacterCode("gordon"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes: [],
                ActiveEventCardCodes:
                [
                    new CardCode("the-hood-interferes"),
                    new CardCode("the-hood-interferes")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(13, result.TargetNumber);
            Assert.Equal(-2, result.TotalBonus);
            Assert.Single(result.AppliedBonuses);
        }

        private static IMediator CreateMediator()
        {
            var fabCards = CreateFabCardsCatalog();
            var eventCards = CreateEventCardsCatalog();

            var services = new ServiceCollection();

            services.AddSingleton<IFabCardDefinitionCatalog>(fabCards);
            services.AddSingleton<IEventCardDefinitionCatalog>(eventCards);
            services.AddRules();
            services.AddFakeCatalogs();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }

        private static FakeFabCardDefinitionCatalog CreateFabCardsCatalog()
        {
            var underwaterSealingUnit = new ReferenceFabCardDefinition(
                code: new CardCode("underwater-sealing-unit"),
                displayName: "Underwater Sealing Unit"
            );

            var jeffsOrders = new ReferenceFabCardDefinition(
                code: new CardCode("jeff-s-orders"),
                displayName: "Jeff's Orders"
            );

            return new FakeFabCardDefinitionCatalog(underwaterSealingUnit, jeffsOrders);
        }

        private static FakeEventCardDefinitionCatalog CreateEventCardsCatalog()
        {
            var theHoodInterferes = new ReferenceEventCardDefinition(
                code: new CardCode("the-hood-interferes"),
                displayName: "The Hood Interferes"
            );

            var explosionOnTracyIsland = new ReferenceEventCardDefinition(
                code: new CardCode("explosion-on-tracy-island"),
                displayName: "Explosion on Tracy Island"
            );

            return new FakeEventCardDefinitionCatalog(theHoodInterferes, explosionOnTracyIsland);
        }
    }
}
