using ThunderbirdsBoardGameEngine.Client.Infrastructure;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;

namespace ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1
{
    public interface IRescueClient
    {
        Task<ApiResult<CalculateRescueTargetResponseDto>> CalculateRescueTargetAsync(string disasterCardCode, CalculateRescueTargetRequestDto request, CancellationToken cancellationToken = default);
    }
}