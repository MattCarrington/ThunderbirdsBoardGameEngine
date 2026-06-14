using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations
{
    public record FindAccessibleLocationsResponse(IReadOnlyCollection<LocationCode> AccessibleLocations);
}
