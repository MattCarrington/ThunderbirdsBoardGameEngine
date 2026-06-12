using Bunit;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.Movement;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.Movement
{
    public class MovementPageTests : BunitContext
    {
        [Fact]
        public void CallsAppropriateServicesOnInitialization()
        {
            // Arrange
            var context = CreateTestContext();

            // Act
            _ = Render<MovementPage>();

            // Assert
            context.ThunderbirdService.Received(1).GetAllMobileVehicles();
            context.LocationService.Received(1).GetAll();
        }

        [Fact]
        public void ValidationButtonIsHiddenWhenThunderbirdNotSelected()
        {
            // Arrange
            var context = CreateTestContext();

            // Act
            var cut = Render<MovementPage>();

            // Assert
            Assert.DoesNotContain(cut.Markup, "Validate Movement");
        }

        [Fact]
        public void ValidationButtonIsDisabledWhenStartLocationNotSelected()
        {
            // Arrange
            var context = CreateTestContext();

            context.ThunderbirdService.GetAllMobileVehicles().Returns(CreateThunderbirdMovementOptions());
            context.LocationService.GetAll().Returns(CreateMovementLocationOptions());

            var cut = Render<MovementPage>();

            // Act
            cut.Find("#thunderbirdSelector").Change("TB2");
            cut.Find("#destination").Change("L1");

            // Assert
            var validateButton = cut.Find("button");
            Assert.True(validateButton.HasAttribute("disabled"));
        }

        [Fact]
        public void ValidationButtonIsDisabledWhenDestinationLocationNotSelected()
        {
            // Arrange
            var context = CreateTestContext();

            context.ThunderbirdService.GetAllMobileVehicles().Returns(CreateThunderbirdMovementOptions());
            context.LocationService.GetAll().Returns(CreateMovementLocationOptions());

            var cut = Render<MovementPage>();

            // Act
            cut.Find("#thunderbirdSelector").Change("TB2");
            cut.Find("#startLocation").Change("L1");

            // Assert
            var validateButton = cut.Find("button");
            Assert.True(validateButton.HasAttribute("disabled"));
        }

        [Fact]
        public void ValidationButtonIsEnabledWhenThunderbirdAndLocationsSelected()
        {
            // Arrange
            var context = CreateTestContext();

            context.ThunderbirdService.GetAllMobileVehicles().Returns(CreateThunderbirdMovementOptions());
            context.LocationService.GetAll().Returns(CreateMovementLocationOptions());

            var cut = Render<MovementPage>();

            // Act
            cut.Find("#thunderbirdSelector").Change("TB2");
            cut.Find("#startLocation").Change("L1");
            cut.Find("#destination").Change("L2");

            // Assert
            Assert.False(cut.Find("button").HasAttribute("disabled"));
        }

        [Fact]
        public async Task CallsMovementServiceOnValidationAsync()
        {
            // Arrange
            var context = CreateTestContext();

            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("TB2");
            cut.Find("#startLocation").Change("L1");
            cut.Find("#destination").Change("L2");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            await context.MovementService.Received(1).ValidateMovementAsync(
                Arg.Is<string>(x => x == "TB2"),
                Arg.Is<string>(x => x == "L1"),
                Arg.Is<string>(x => x == "L2")
            );
        }

        [Fact]
        public void DisplaysValidationResultWhenResponseIsSuccessful()
        {
            // Arrange
            var response = new MovementResultViewModel(
                IsValid: true,
                ActionPointCost: 2,
                SpacesTravelled: 3,
                TopSpeed: 150,
                Route: ["L1", "L3", "L2"],
                Messages: ["Movement Validated!"]);

            var context = CreateTestContext();

            context.MovementService.ValidateMovementAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(response);

            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("TB2");
            cut.Find("#startLocation").Change("L1");
            cut.Find("#destination").Change("L2");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='validation-success']");
            });
        }

        [Fact]
        public void DisplaysValidationFailedWhenResponseIsNull()
        {
            // Arrange
            var context = CreateTestContext();

            context.MovementService.ValidateMovementAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns((MovementResultViewModel?)null);

            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("TB2");
            cut.Find("#startLocation").Change("L1");
            cut.Find("#destination").Change("L2");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                cut.Find("[data-testid='validation-error']");
            });
        }

        private MovementPageTestContext CreateTestContext()
        {
            var context = new MovementPageTestContext(this);

            context.ThunderbirdService.GetAllMobileVehicles().Returns(CreateThunderbirdMovementOptions());
            context.LocationService.GetAll().Returns(CreateMovementLocationOptions());

            return context;
        }

        private static IReadOnlyList<ThunderbirdMovementOptions> CreateThunderbirdMovementOptions()
        {
            return
            [
                new(Key: "TB1", DisplayName: "Thunderbird 1"),
                new(Key: "TB2", DisplayName: "Thunderbird 2"),
            ];
        }

        private static IReadOnlyList<MovementLocationOptions> CreateMovementLocationOptions()
        {
            return
            [
                new(Key: "L1", DisplayName: "Location 1"),
                new(Key: "L2", DisplayName: "Location 2"),
            ];
        }
    }
}
