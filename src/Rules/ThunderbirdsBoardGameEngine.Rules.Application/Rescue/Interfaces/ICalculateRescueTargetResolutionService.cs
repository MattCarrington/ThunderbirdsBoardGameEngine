using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Calculation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface ICalculateRescueTargetResolutionService
    {
        RescueTargetResult ResolveRescueCalculation(RescueCalculationRequest request);
    }
}