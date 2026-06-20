using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services
{
    public class RescueClientService : IRescueClientService
    {
        private readonly IRescueClient _rescueClient;

        public RescueClientService(IRescueClient rescueClient)
        {
            _rescueClient = rescueClient;
        }

        public async Task<CalculateRescueTargetResponseDto?> CalculateRescueTargetAsync(
            string disasterCardCode,
            IReadOnlyCollection<string> presentBonusKeys,
            string performingCharacterKey)
        {
            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = presentBonusKeys,
                PerformingCharacterKey = performingCharacterKey
            };

            var result = await _rescueClient.CalculateRescueTargetAsync(disasterCardCode, request);

            return result.Success ? result.Data : null;
        }
    }
}
