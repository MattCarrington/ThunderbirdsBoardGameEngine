using Bunit;
using Microsoft.Extensions.Configuration;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.Rules.WireMock;
using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using ThunderbirdsBoardGameEngine.UI.Features.Movement;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests.Pages
{
    [Collection("WireMock")]
    public class MovementPageTests : BunitContext
    {
        private readonly MovementStub _movementStub;

        public MovementPageTests(WireMockFixture wireMockFixture)
        {
            var host = wireMockFixture.Host;
            host.Reset();

            _movementStub = host.MovementStub();
            _movementStub.RegisterMissingHeaderGuard();
            _movementStub.RegisterIncorrectHeaderGuard();

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "RulesClient:BaseAddress", host.Url }
                })
                .Build();

            Services.AddReferenceData();
            Services.AddRulesClients(configuration);
            Services.AddUiServices();
        }

        [Fact]
        public void LoadsThunderbirdsFromReferenceData()
        {
            // Arrange

            // Act
            var cut = Render<MovementPage>();

            // Assert
            var cardOptions = cut
                .FindAll("#thunderbirdSelector option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            Assert.Equal(5, cardOptions.Count - 1); // Subtract 1 for the default "Select a Thunderbird" option
        }

        [Fact]
        public void LoadsMovementLocationsWhenThunderbirdSelected()
        {
            // Arrange
            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("fab-1");

            // Act
            var locationOptions = cut
                .FindAll("#startLocation option")
                .Select(o => o.TextContent.Trim())
                .ToList();

            // Assert
            Assert.Equal(18, locationOptions.Count - 1); // Subtract 1 for the default "Select a Location" option
        }

        [Fact]
        public void DisplaysValidationSuccessMovementResultWhenSuccessResponseIsValid()
        {
            // Arrange
            var response = new ValidateMovementResponseDto
            {
                IsValid = true,
                ActionPointCost = 1,
                SpacesTravelled = 1,
                TopSpeed = 1,
                Route = ["Location A", "Location B", "Location C"],
                Messages = []
            };

            _movementStub.RegisterValidateMovementSuccess(response);

            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("fab-1");
            cut.Find("#startLocation").Change("asia");
            cut.Find("#destination").Change("europe");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                var result = cut.Find("[data-testid='validation-success']");
                Assert.Contains(response.ActionPointCost.ToString(), result.TextContent);
                Assert.Contains(response.SpacesTravelled.ToString(), result.TextContent);

                Assert.Empty(cut.FindAll("[data-testid='validation-failure']"));    // No invalid route message
                Assert.Empty(cut.FindAll("[data-testid='validation-error']"));  // No error message
            },
            timeout: TimeSpan.FromSeconds(5));  // Increased timeout for WireMock HTTP call
        }

        [Fact]
        public void DisplaysValidationFailureMovementResultWhenSuccessResponseIsInvalid()
        {
            // Arrange
            var response = new ValidateMovementResponseDto
            {
                IsValid = false,
                ActionPointCost = 0,
                SpacesTravelled = 0,
                TopSpeed = 0,
                Route = [],
                Messages = ["No route found between asia and europe for thunderbird-3"]
            };

            _movementStub.RegisterValidateMovementSuccess(response);

            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("thunderbird-3");
            cut.Find("#startLocation").Change("asia");
            cut.Find("#destination").Change("europe");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                var result = cut.Find("[data-testid='validation-failure']");
                Assert.Contains(response.Messages.First(), result.TextContent);

                Assert.Empty(cut.FindAll("[data-testid='validation-success']"));    // No success message
                Assert.Empty(cut.FindAll("[data-testid='validation-error']"));  // No error message
            },
            timeout: TimeSpan.FromSeconds(5));  // Increased timeout for WireMock HTTP call
        }

        [Fact]
        public void DisplaysValidationErrorWhenResponseIsError()
        {
            // Arrange
            _movementStub.RegisterValidateMovementError();

            var cut = Render<MovementPage>();

            cut.Find("#thunderbirdSelector").Change("thunderbird-4");
            cut.Find("#startLocation").Change("asia");
            cut.Find("#destination").Change("europe");

            // Act
            cut.Find("[data-testid='calculate-button']").Click();

            // Assert
            cut.WaitForAssertion(() =>
            {
                var result = cut.Find("[data-testid='validation-error']");
                Assert.Contains("There was an error validating the movement. Please check the inputs and try again.", result.TextContent);

                Assert.Empty(cut.FindAll("[data-testid='validation-success']"));    // No success message
                Assert.Empty(cut.FindAll("[data-testid='validation-failure']"));  // No invalid route message
            },
            timeout: TimeSpan.FromSeconds(5));  // Increased timeout for WireMock HTTP call
        }
    }
}
