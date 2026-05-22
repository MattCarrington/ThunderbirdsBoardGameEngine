using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface ICalculateRescueTargetResolutionService
    {
        RescueTargetResult ResolveRescueCalculation(RescueCalculationRequest request);
    }
}