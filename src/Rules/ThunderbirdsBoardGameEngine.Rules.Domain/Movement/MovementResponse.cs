using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public record MovementResponse(bool IsValid, int SpacesTravelled, IReadOnlyList<LocationCode> Route, int ActionPointCost, int TopSpeed, IReadOnlyList<string> Messages);
}
