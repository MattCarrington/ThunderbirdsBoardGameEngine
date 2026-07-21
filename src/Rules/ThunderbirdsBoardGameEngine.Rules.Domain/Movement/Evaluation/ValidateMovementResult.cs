using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation
{
    public record ValidateMovementResult(
        bool IsValid,
        int SpacesTravelled,
        IReadOnlyCollection<LocationCode> Route,
        int ActionPointCost,
        int? EffectiveTopSpeed,
        int ThunderbirdTopSpeed,
        IReadOnlyCollection<string> Messages);
}
