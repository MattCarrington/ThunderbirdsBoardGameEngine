using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations
{
    public record FindAccessibleLocationsResponse(IReadOnlyCollection<LocationCode> AccessibleLocations);
}
