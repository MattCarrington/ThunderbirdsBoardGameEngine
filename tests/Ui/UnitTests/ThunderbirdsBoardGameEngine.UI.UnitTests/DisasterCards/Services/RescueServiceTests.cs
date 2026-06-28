using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.DisasterCards.Services
{
    public class RescueServiceTests
    {
        [Fact]
        public async Task CalculateRescueTargetAsync_WhenResponseIsSuccessful_ReturnsCalculateRescueTargetResponseDto()
        {
            // Arrange
            var parameters = CreateRequestParameters();

            var expectedResponse = new CalculateRescueTargetResponseDto
            {
                TargetNumber = 10,
                TotalBonus = 5,
                AppliedDisasterBonuses = []
            };

            var apiResult = ApiResult<CalculateRescueTargetResponseDto>.SuccessResult(expectedResponse, HttpStatusCode.OK);

            var rescueClient = CreateRescueClient(apiResult);

            var service = CreateRescueService(rescueClient);

            // Act
            var result = await service.CalculateRescueTargetAsync(
                parameters.DisasterCardCode,
                parameters.PresentBonusKeys,
                parameters.PerformingCharacterKey,
                parameters.PresentFabCardKeys,
                parameters.ActiveEventCardKeys);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse, result);

            var expectedRequest = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = parameters.PresentBonusKeys,
                PerformingCharacterKey = parameters.PerformingCharacterKey,
                PlayedFabCardKeys = parameters.PresentFabCardKeys
            };

            await rescueClient.Received(1)
                .CalculateRescueTargetAsync(
                    Arg.Is(parameters.DisasterCardCode),
                    Arg.Is(expectedRequest),
                    Arg.Any<CancellationToken>()
                );
        }

        [Fact]
        public async Task CalculateRescueTargetAsync_WhenResponseIsNotSuccessful_ReturnsNull()
        {
            // Arrange
            var parameters = CreateRequestParameters();

            var apiResult = ApiResult<CalculateRescueTargetResponseDto>.Failure("Error", HttpStatusCode.BadRequest);

            var rescueClient = CreateRescueClient(apiResult);

            var service = CreateRescueService(rescueClient);

            // Act
            var result = await service.CalculateRescueTargetAsync(
                parameters.DisasterCardCode,
                parameters.PresentBonusKeys,
                parameters.PerformingCharacterKey,
                parameters.PresentFabCardKeys,
                parameters.ActiveEventCardKeys);

            // Assert
            Assert.Null(result);
        }

        private static RequestParameters CreateRequestParameters()
        {
            var disasterCardCode = "DISASTER_001";

            var presentBonusKeys = new[] { "BONUS_001", "BONUS_002" };

            var performingCharacterKey = "CHARACTER_001";

            return new RequestParameters(disasterCardCode, presentBonusKeys, performingCharacterKey, Array.Empty<string>(), Array.Empty<string>());
        }

        private static IRescueClient CreateRescueClient(ApiResult<CalculateRescueTargetResponseDto> apiResult)
        {
            var rescueClient = Substitute.For<IRescueClient>();
            rescueClient.CalculateRescueTargetAsync(Arg.Any<string>(), Arg.Any<CalculateRescueTargetRequestDto>())
                .Returns(apiResult);

            return rescueClient;
        }

        private static RescueClientService CreateRescueService(IRescueClient rescueClient)
        {
            return new RescueClientService(rescueClient);
        }

        private record RequestParameters(
            string DisasterCardCode,
            IReadOnlyCollection<string> PresentBonusKeys,
            string PerformingCharacterKey,
            IReadOnlyCollection<string> PresentFabCardKeys,
            IReadOnlyCollection<string> ActiveEventCardKeys);
    }
}
