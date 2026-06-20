using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces
{
    public interface IRescueClientService
    {
        Task<CalculateRescueTargetResponseDto?> CalculateRescueTargetAsync(string disasterCardCode, IReadOnlyCollection<string> presentBonusKeys, string performingCharacterKey);
    }
}