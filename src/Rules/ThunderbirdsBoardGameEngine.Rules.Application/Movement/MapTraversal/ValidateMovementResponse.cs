using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public record ValidateMovementResponse(bool IsValid, int SpacesTravelled, IReadOnlyList<LocationCode> Route, int ActionPointCost, int TopSpeed, IReadOnlyList<string> Messages);
}
