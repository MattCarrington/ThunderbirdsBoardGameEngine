using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Controllers.Rules.V1;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Controllers.Rules.V1
{
    public class RescueControllerTests
    {
        [Fact]
        public async Task CalculateRescueTarget_WhenCalled_ReturnsOkAsync()
        {
            // Arrange
            var cardCode = "card-code";

            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = []
            };

            var mediator = Substitute.For<IMediator>();
            mediator.Send(Arg.Any<CalculateRescueTargetQuery>())
                .Returns(new CalculateRescueTargetResponse(11, 0, []));

            var controller = CreateController(mediator);

            // Act
            var result = await controller.CalculateRescueTarget(cardCode, request, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var target = Assert.IsType<CalculateRescueTargetResponseDto>(okResult.Value);
            Assert.NotNull(target);

            await mediator.Received(1).Send(Arg.Any<CalculateRescueTargetQuery>());
        }

        private static RescueController CreateController(IMediator mediator)
        {
            return new RescueController(mediator);
        }
    }
}
