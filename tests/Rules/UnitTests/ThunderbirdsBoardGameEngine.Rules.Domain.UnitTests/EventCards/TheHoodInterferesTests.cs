using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.EventCards
{
    public class TheHoodInterferesTests
    {
        [Theory]
        [InlineData(RescueType.Air)]
        [InlineData(RescueType.Land)]
        [InlineData(RescueType.Sea)]
        [InlineData(RescueType.Space)]
        public void ApplyRescueModifier_WhenApplied_ReturnsMinusTwoModifierRegardlessOfRescueType(RescueType rescueType)
        {
            // Arrange
            var theHoodInterferes = new TheHoodInterferes();

            var input = new RescueCalculationInput(Array.Empty<DisasterBonusKey>(), rescueType);

            // Act
            var result = theHoodInterferes.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single(result);
            Assert.Equal("the-hood-interferes", bonus.Key);
            Assert.Equal(-2, bonus.Value);
            Assert.Equal(SourceType.EventCard, bonus.SourceType);
        }
    }
}