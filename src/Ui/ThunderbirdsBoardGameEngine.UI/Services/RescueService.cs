using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class RescueService : IRescueService
    {
        private readonly IRescueClient _rescueClient;

        public RescueService(IRescueClient rescueClient)
        {
            _rescueClient = rescueClient;
        }

        public async Task<CalculateRescueTargetResponseDto?> CalculateRescueTargetAsync(string disasterCardCode, IReadOnlyCollection<string> presentBonusKeys)
        {
            var request = new CalculateRescueTargetRequestDto
            {
                PresentDisasterBonusKeys = presentBonusKeys,
                PerformingCharacterKey = string.Empty   // TODO: Update when character selection is implemented
            };

            var result = await _rescueClient.CalculateRescueTargetAsync(disasterCardCode, request);

            return result.Success ? result.Data : null;
        }
    }
}
