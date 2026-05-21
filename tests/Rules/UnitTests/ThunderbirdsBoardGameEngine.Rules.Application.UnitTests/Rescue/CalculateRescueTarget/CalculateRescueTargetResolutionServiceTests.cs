using NSubstitute;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetResolutionServiceTests
    {
        private readonly IDisasterCatalogLookup _disasterContributionLookup;
        private readonly ICharacterCatalogLookup _characterContributionLookup;
        private readonly IFabCardCatalogLookup _fabCardCatalogLookup;
        private readonly IEventCardCatalogLookup _eventCardCatalogLookup;
        private readonly IBonusModifierSourceRegistry _bonusModifierSourceRegistry;
        private readonly RescueTargetCalculator _rescueTargetCalculator;
        private readonly CalculateRescueTargetResolutionService _sut;

        public CalculateRescueTargetResolutionServiceTests()
        {
            _disasterContributionLookup = Substitute.For<IDisasterCatalogLookup>();
            _characterContributionLookup = Substitute.For<ICharacterCatalogLookup>();
            _fabCardCatalogLookup = Substitute.For<IFabCardCatalogLookup>();
            _eventCardCatalogLookup = Substitute.For<IEventCardCatalogLookup>();
            _bonusModifierSourceRegistry = Substitute.For<IBonusModifierSourceRegistry>();
            _rescueTargetCalculator = new RescueTargetCalculator();
            _sut = new CalculateRescueTargetResolutionService(
                _disasterContributionLookup,
                _characterContributionLookup,
                _fabCardCatalogLookup,
                _eventCardCatalogLookup,
                _bonusModifierSourceRegistry,
                _rescueTargetCalculator);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithNoFabCards_ReturnsResultWithDisasterAndCharacterContributions()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardsCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardsCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TargetRoll);
            _disasterContributionLookup.Received(1).GetDisasterRescueContribution(disasterCardCode);
            _characterContributionLookup.Received(1).GetCharacterRescueContribution(characterCode);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithSingleFabCard_IncludesFabCardInSources()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var fabCardCode = new CardCode("fab-card-1");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = new[] { fabCardCode };
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            var fabCardSource = Substitute.For<IRescueModifierSource>();
            fabCardSource.ApplyRescueModifier(Arg.Any<RescueCalculationInput>()).Returns(Array.Empty<AppliedRescueModifier>());

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(fabCardCode, out Arg.Any<IRescueModifierSource>())
                .Returns(x =>
                {
                    x[1] = fabCardSource;
                    return true;
                });

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode, out Arg.Any<IRescueModifierSource>());
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithMultipleFabCards_IncludesAllFoundFabCardsInSources()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var fabCardCode1 = new CardCode("fab-card-1");
            var fabCardCode2 = new CardCode("fab-card-2");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = new[] { fabCardCode1, fabCardCode2 };
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            var fabCardSource1 = Substitute.For<IRescueModifierSource>();
            fabCardSource1.ApplyRescueModifier(Arg.Any<RescueCalculationInput>()).Returns(Array.Empty<AppliedRescueModifier>());

            var fabCardSource2 = Substitute.For<IRescueModifierSource>();
            fabCardSource2.ApplyRescueModifier(Arg.Any<RescueCalculationInput>()).Returns(Array.Empty<AppliedRescueModifier>());

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(fabCardCode1, out Arg.Any<IRescueModifierSource>())
                .Returns(x =>
                {
                    x[1] = fabCardSource1;
                    return true;
                });
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(fabCardCode2, out Arg.Any<IRescueModifierSource>())
                .Returns(x =>
                {
                    x[1] = fabCardSource2;
                    return true;
                });

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode1, out Arg.Any<IRescueModifierSource>());
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode2, out Arg.Any<IRescueModifierSource>());
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithUnknownFabCard_SkipsFabCardNotInRegistry()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var fabCardCode = new CardCode("unknown-fab-card");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = new[] { fabCardCode };
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(fabCardCode, out Arg.Any<IRescueModifierSource>())
                .Returns(false);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TargetRoll);
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode, out Arg.Any<IRescueModifierSource>());
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithMixedKnownAndUnknownFabCards_IncludesOnlyKnownFabCards()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var knownFabCardCode = new CardCode("known-fab-card");
            var unknownFabCardCode = new CardCode("unknown-fab-card");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = new[] { knownFabCardCode, unknownFabCardCode };
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            var knownFabCardSource = Substitute.For<IRescueModifierSource>();
            knownFabCardSource.ApplyRescueModifier(Arg.Any<RescueCalculationInput>()).Returns(Array.Empty<AppliedRescueModifier>());

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(knownFabCardCode, out Arg.Any<IRescueModifierSource>())
                .Returns(x =>
                {
                    x[1] = knownFabCardSource;
                    return true;
                });
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(unknownFabCardCode, out Arg.Any<IRescueModifierSource>())
                .Returns(false);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(knownFabCardCode, out Arg.Any<IRescueModifierSource>());
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(unknownFabCardCode, out Arg.Any<IRescueModifierSource>());
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithDisasterBonusKeys_PassesDisasterBonusKeysToCalculationInput()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var disasterBonusKey1 = new DisasterBonusKey("bonus-1");
            var disasterBonusKey2 = new DisasterBonusKey("bonus-2");
            var disasterBonusKeys = new[] { disasterBonusKey1, disasterBonusKey2 };
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterBonus1 = new DisasterBonus(disasterBonusKey1, 2);
            var disasterBonus2 = new DisasterBonus(disasterBonusKey2, 3);
            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: new[] { disasterBonus1, disasterBonus2 },
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TargetRoll);
            Assert.Equal(5, result.TotalBonus);
            Assert.Equal(2, result.AppliedModifiers.Count);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_WithCharacterBonus_AppliesCharacterBonusWhenRescueTypeMatches()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterRescueBonus = new CharacterRescueBonusContribution(RescueType.Air, 3);
            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonus);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(7, result.TargetRoll);
            Assert.Equal(3, result.TotalBonus);
            Assert.Single(result.AppliedModifiers);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_PassesRescueTypeFromDisasterToCalculationInput()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Sea);

            var characterRescueBonusAir = new CharacterRescueBonusContribution(RescueType.Air, 3);
            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusAir);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TargetRoll);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedModifiers);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_CallsDisasterContributionLookupWithCorrectParameter()
        {
            // Arrange
            var disasterCardCode = new CardCode("specific-disaster");
            var characterCode = new CharacterCode("test-character");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            _sut.ResolveRescueCalculationAsync(request);

            // Assert
            _disasterContributionLookup.Received(1).GetDisasterRescueContribution(disasterCardCode);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_CallsCharacterContributionLookupWithCorrectParameter()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("specific-character");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            _sut.ResolveRescueCalculationAsync(request);

            // Assert
            _characterContributionLookup.Received(1).GetCharacterRescueContribution(characterCode);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_PassesDifficultyNumberFromDisasterToCalculator()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = Array.Empty<CardCode>();
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 15,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);

            // Act
            var result = _sut.ResolveRescueCalculationAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TargetRoll);
        }

        [Fact]
        public void ResolveRescueCalculationAsync_IteratesAllFabCards_ProcessesEachFabCardInOrder()
        {
            // Arrange
            var disasterCardCode = new CardCode("test-disaster");
            var characterCode = new CharacterCode("test-character");
            var fabCardCode1 = new CardCode("fab-card-1");
            var fabCardCode2 = new CardCode("fab-card-2");
            var fabCardCode3 = new CardCode("fab-card-3");
            var disasterBonusKeys = Array.Empty<DisasterBonusKey>();
            var fabCardCodes = new[] { fabCardCode1, fabCardCode2, fabCardCode3 };
            var eventCardCodes = Array.Empty<CardCode>();

            var request = new RescueCalculationRequest(
                disasterCardCode,
                characterCode,
                disasterBonusKeys,
                fabCardCodes,
                eventCardCodes);

            var disasterContribution = new DisasterContribution(
                difficultyNumber: 10,
                availableBonuses: Array.Empty<DisasterBonus>(),
                rescueType: RescueType.Air);

            var characterContribution = new CharacterContribution(
                characterCode,
                characterRescueBonusContribution: null);

            _disasterContributionLookup.GetDisasterRescueContribution(disasterCardCode).Returns(disasterContribution);
            _characterContributionLookup.GetCharacterRescueContribution(characterCode).Returns(characterContribution);
            _bonusModifierSourceRegistry.TryGetBonusModifierSource(Arg.Any<CardCode>(), out Arg.Any<IRescueModifierSource>())
                .Returns(false);

            // Act
            _sut.ResolveRescueCalculationAsync(request);

            // Assert
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode1, out Arg.Any<IRescueModifierSource>());
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode2, out Arg.Any<IRescueModifierSource>());
            _bonusModifierSourceRegistry.Received(1).TryGetBonusModifierSource(fabCardCode3, out Arg.Any<IRescueModifierSource>());
        }
    }
}
