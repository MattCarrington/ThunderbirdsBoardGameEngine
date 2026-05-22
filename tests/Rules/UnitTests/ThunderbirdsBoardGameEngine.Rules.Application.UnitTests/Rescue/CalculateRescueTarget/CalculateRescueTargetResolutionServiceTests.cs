using NSubstitute;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetResolutionServiceTests
    {
        private static readonly CardCode ValidFabCard = new("valid-fab-card");
        private static readonly CardCode ValidEventCard = new("valid-event-card");

        [Fact]
        public void ResolveRescueCalculation_WhenPlayedFabCardDoesNotExist_ThrowsInvalidRescueCalculationRequestException()
        {
            // Arrange
            var request = CreateValidRequest(
                fabCards: [new CardCode("nonexistent-fab-card")],
                eventCards: []);

            var fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();
            fabCardCatalogLookup.Exists(Arg.Any<CardCode>()).Returns(false);

            var eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            var bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();

            var service = CreateService(fabCardCatalogLookup, eventCardCatalogLookup, bonusModifierSourceRegistry);

            // Act & Assert
            Assert.Throws<InvalidRescueCalculationRequestException>(() => service.ResolveRescueCalculation(request));
        }

        [Fact]
        public void ResolveRescueCalculation_WhenActiveEventCardDoesNotExist_ThrowsInvalidRescueCalculationRequestException()
        {
            // Arrange
            var request = CreateValidRequest(
                fabCards: [],
                eventCards: [new CardCode("nonexistent-event-card")]);

            var fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();

            var eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            eventCardCatalogLookup.Exists(Arg.Any<CardCode>()).Returns(false);

            var bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();

            var service = CreateService(fabCardCatalogLookup, eventCardCatalogLookup, bonusModifierSourceRegistry);

            // Act & Assert
            Assert.Throws<InvalidRescueCalculationRequestException>(() => service.ResolveRescueCalculation(request));
        }

        [Fact]
        public void ResolveRescueCalculation_WhenNoFabOrEventCards_DoesNotValidateOrQueryCardModifiers()
        {
            // Arrange
            var request = CreateValidRequest(
                fabCards: [],
                eventCards: []);

            var fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();
            var eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            var bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();

            var service = CreateService(fabCardCatalogLookup, eventCardCatalogLookup, bonusModifierSourceRegistry);

            // Act
            service.ResolveRescueCalculation(request);

            // Assert
            fabCardCatalogLookup.DidNotReceiveWithAnyArgs().Exists(Arg.Any<CardCode>());
            eventCardCatalogLookup.DidNotReceiveWithAnyArgs().Exists(Arg.Any<CardCode>());
            bonusModifierSourceRegistry.DidNotReceiveWithAnyArgs().TryGetBonusModifierSource(Arg.Any<CardCode>(), out Arg.Any<IRescueModifierSource>());
        }

        [Fact]
        public void ResolveRescueCalculation_WhenValidFabAndEventCards_AsksModifierRegistryForEachCard()
        {
            // Arrange
            var request = CreateValidRequest(
                fabCards: [ValidFabCard],
                eventCards: [ValidEventCard]);

            var fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();
            fabCardCatalogLookup.Exists(Arg.Is(ValidFabCard)).Returns(true);

            var eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            eventCardCatalogLookup.Exists(Arg.Is(ValidEventCard)).Returns(true);

            var bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();

            var service = CreateService(fabCardCatalogLookup, eventCardCatalogLookup, bonusModifierSourceRegistry);

            // Act
            service.ResolveRescueCalculation(request);

            // Assert
            bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(
                Arg.Is(ValidFabCard),
                out Arg.Any<IRescueModifierSource>());

            bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(
                Arg.Is(ValidEventCard),
                out Arg.Any<IRescueModifierSource>());
        }

        [Fact]
        public void ResolveRescueCalculation_WhenModifierSourceExists_IncludesReturnedModifiersInCalculation()
        {
            var request = CreateValidRequest(
                    fabCards: [ValidFabCard],
                    eventCards: []);

            var fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();
            fabCardCatalogLookup.Exists(Arg.Is(ValidFabCard)).Returns(true);

            var eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            eventCardCatalogLookup.Exists(Arg.Is(ValidEventCard)).Returns(true);

            var modifierSource = Substitute.For<IRescueModifierSource>();
            modifierSource
                .ApplyRescueModifier(Arg.Any<RescueCalculationInput>())
                .Returns([
                    new AppliedRescueModifier
                    {
                        Key = "test-modifier",
                        Value = 3,
                        SourceType = SourceType.FabCard
                    }
                ]);

            var bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();
            bonusModifierSourceRegistry
                .TryGetBonusModifierSource(
                    Arg.Is(ValidFabCard),
                    out Arg.Any<IRescueModifierSource>())
                .Returns(callInfo =>
                {
                    callInfo[1] = modifierSource;
                    return true;
                });

            var service = CreateService(fabCardCatalogLookup, eventCardCatalogLookup, bonusModifierSourceRegistry);

            // Act
            var result = service.ResolveRescueCalculation(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TargetRoll);
            Assert.Equal(3, result.TotalBonus);

            var appliedModifier = Assert.Single(result.AppliedModifiers);
            Assert.Equal("test-modifier", appliedModifier.Key);
            Assert.Equal(3, appliedModifier.Value);
            Assert.Equal(SourceType.FabCard, appliedModifier.SourceType);
        }

        [Fact]
        public void ResolveRescueCalculation_WhenModifierSourceDoesNotExist_IgnoresCard()
        {
            var request = CreateValidRequest(
                fabCards: [ValidFabCard],
                eventCards: [ValidEventCard]);

            var fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();
            fabCardCatalogLookup.Exists(Arg.Is(ValidFabCard)).Returns(true);

            var eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            eventCardCatalogLookup.Exists(Arg.Is(ValidEventCard)).Returns(true);

            var bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();
            bonusModifierSourceRegistry.TryGetBonusModifierSource(Arg.Any<CardCode>(), out Arg.Any<IRescueModifierSource>()).Returns(false);

            var service = CreateService(fabCardCatalogLookup, eventCardCatalogLookup, bonusModifierSourceRegistry);

            // Act
            var result = service.ResolveRescueCalculation(request);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.AppliedModifiers);
        }

        private static RescueCalculationRequest CreateValidRequest(IReadOnlyCollection<CardCode> fabCards, IReadOnlyCollection<CardCode> eventCards)
        {
            return new RescueCalculationRequest(
                DisasterCardCode: new CardCode("test-disaster"),
                PerformingCharacter: new CharacterCode("test-character"),
                PresentDisasterBonusKeys: [new DisasterBonusKey("test-bonus")],
                PlayedFabCardCodes: fabCards,
                ActiveEventCardCodes: eventCards);
        }

        private static CalculateRescueTargetResolutionService CreateService(
            IFabCardCatalogLookup fabCardCatalogLookup,
            IEventCardCatalogLookup eventCardCatalogLookup,
            IBonusModifierSourceRegistry bonusModifierSourceRegistry)
        {
            var disaster = new DisasterContribution(
                difficultyNumber: 5,
                availableBonuses: [],
                rescueType: RescueType.Land);

            var character = new CharacterContribution(
                key: new CharacterCode("test-character"),
                characterRescueBonusContribution: null);

            var disasterCatalogLookup = Substitute.For<IDisasterCatalogLookup>();
            disasterCatalogLookup.GetDisasterRescueContribution(Arg.Any<CardCode>()).Returns(disaster);

            var characterCatalogLookup = Substitute.For<ICharacterCatalogLookup>();
            characterCatalogLookup.GetCharacterRescueContribution(Arg.Any<CharacterCode>()).Returns(character);

            var rescueTargetCalculator = new RescueTargetCalculator();

            return new CalculateRescueTargetResolutionService(
                disasterCatalogLookup,
                characterCatalogLookup,
                fabCardCatalogLookup,
                eventCardCatalogLookup,
                bonusModifierSourceRegistry,
                rescueTargetCalculator);
        }
    }
}
