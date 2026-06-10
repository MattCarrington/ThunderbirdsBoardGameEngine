using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Movement.Components
{
    public class MovementResultDetailTests : BunitContext
    {
        [Fact]
        public void MovementResultDetailShouldRenderCorrectlyWhenResultIsValid()
        {
            // Arrange
            var movementResult = CreateMovementResultViewModel(isValid: true);

            // Act
            var cut = Render<MovementResultDetail>(parameters => parameters
                .Add(p => p.MovementResult, movementResult)
            );

            // Assert
            var actionPointCost = cut.Find("[data-testid='action-point-cost']");
            Assert.Contains("3", actionPointCost.TextContent);

            var spacesTravelled = cut.Find("[data-testid='spaces-travelled']");
            Assert.Contains("5", spacesTravelled.TextContent);

            var route = cut.Find("[data-testid='route']");
            Assert.Contains("A1", route.TextContent);
            Assert.Contains("A2", route.TextContent);
            Assert.Contains("A3", route.TextContent);
            Assert.Contains("A4", route.TextContent);
            Assert.Contains("A5", route.TextContent);

            var validationFailed = cut.FindAll("[data-testid='validation-warning']");
            Assert.Empty(validationFailed);
        }

        [Fact]
        public void MovementResultDetailShouldRenderCorrectlyWhenResultIsInvalid()
        {
            // Arrange
            var movementResult = CreateMovementResultViewModel(isValid: false);

            // Act
            var cut = Render<MovementResultDetail>(parameters => parameters
                .Add(p => p.MovementResult, movementResult)
            );

            // Assert
            var message = cut.Find("[data-testid='validation-failure']");
            Assert.Contains("Movement failed due to invalid route.", message.TextContent);

            var validationSuccess = cut.FindAll("[data-testid='validation-success']");
            Assert.Empty(validationSuccess);

        }

        private static MovementResultViewModel CreateMovementResultViewModel(bool isValid)
        {
            return new MovementResultViewModel
            (
                IsValid: isValid,
                ActionPointCost: 3,
                SpacesTravelled: 5,
                TopSpeed: 2,
                Route: ["A1", "A2", "A3", "A4", "A5"],
                Messages: isValid ? ["Movement successful!"] : ["Movement failed due to invalid route."]
            );
        }
    }
}
