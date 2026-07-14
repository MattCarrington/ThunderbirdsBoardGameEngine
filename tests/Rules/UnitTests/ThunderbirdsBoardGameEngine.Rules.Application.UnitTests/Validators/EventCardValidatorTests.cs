using NSubstitute;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Validators;

namespace ThunderbirdsBoardGameEngine.Rules.Application.UnitTests.Validators
{
    public class EventCardValidatorTests
    {
        [Fact]
        public void Validate_ShouldThrowReferenceDataNotFoundException_WhenEventCardDoesNotExist()
        {
            // Arrange
            var lookup = Substitute.For<IEventCardCatalogLookup>();
            lookup.Exists(Arg.Any<CardCode>()).Returns(false);

            var validator = new EventCardValidator(lookup);

            var eventCards = new List<CardCode> { new("NonExistentCard") };

            // Act & Assert
            Assert.Throws<ReferenceDataNotFoundException>(() => validator.Validate(eventCards));

            AssertLookupExistsCalledForAllCards(lookup, eventCards);
        }

        [Fact]
        public void Validate_ShouldNotThrowException_WhenEventCardExists()
        {
            // Arrange
            var lookup = Substitute.For<IEventCardCatalogLookup>();
            lookup.Exists(Arg.Any<CardCode>()).Returns(true);

            var validator = new EventCardValidator(lookup);

            var eventCards = new List<CardCode>
            {
                KnownEventCardCodes.AttackOfTheZombites,
                KnownEventCardCodes.UsnSentinelMissileStrike,
                KnownEventCardCodes.RocketMalfunction
            };

            // Act
            var exception = Record.Exception(() => validator.Validate(eventCards));

            // Assert
            Assert.Null(exception);

            AssertLookupExistsCalledForAllCards(lookup, eventCards);
        }

        private void AssertLookupExistsCalledForAllCards(IEventCardCatalogLookup lookup, IReadOnlyCollection<CardCode> eventCards)
        {
            foreach (var cardCode in eventCards)
            {
                lookup.Received(1).Exists(cardCode);
            }
        }
    }
}
