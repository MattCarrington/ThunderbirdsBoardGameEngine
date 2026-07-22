using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public record ValidateMovementResponse(
        bool IsValid,
        int SpacesTravelled,
        IReadOnlyCollection<LocationCode> Route,
        int ActionPointCost,
        int? EffectiveTopSpeed,
        int ThunderbirdTopSpeed,
        IReadOnlyCollection<string> Messages);
}
