using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Movement
{
    public class ActionPointCalculatorTests
    {
        [Theory]
        [InlineData(0, 1, 0)]
        [InlineData(0, 2, 0)]
        [InlineData(0, 3, 0)]
        [InlineData(1, 1, 1)]
        [InlineData(1, 2, 1)]
        [InlineData(1, 3, 1)]
        [InlineData(2, 1, 2)]
        [InlineData(2, 2, 1)]
        [InlineData(2, 3, 1)]
        [InlineData(3, 1, 3)]
        [InlineData(3, 2, 2)]
        [InlineData(3, 3, 1)]
        [InlineData(4, 1, 4)]
        [InlineData(4, 2, 2)]
        [InlineData(4, 3, 2)]
        [InlineData(5, 1, 5)]
        [InlineData(5, 2, 3)]
        [InlineData(5, 3, 2)]
        public void CalculateActionPoints_ForValidMovement_ReturnsExpectedPoints(int spacesTravelled, int topSpeed, int expectedPoints)
        {
            // Arrange
            var calculator = new ActionPointCalculator();

            // Act
            var result = calculator.CalculateActionPoints(spacesTravelled, topSpeed);

            // Assert
            Assert.Equal(expectedPoints, result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CalculateActionPoints_ForNegativeSpacesTravelled_ThrowsArgumentOutOfRangeException(int spacesTravelled)
        {
            // Arrange
            var calculator = new ActionPointCalculator();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculateActionPoints(spacesTravelled, 1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void CalculateActionPoints_ForNonPositiveTopSpeed_ThrowsArgumentOutOfRangeException(int topSpeed)
        {
            // Arrange
            var calculator = new ActionPointCalculator();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => calculator.CalculateActionPoints(1, topSpeed));
        }
    }
}
