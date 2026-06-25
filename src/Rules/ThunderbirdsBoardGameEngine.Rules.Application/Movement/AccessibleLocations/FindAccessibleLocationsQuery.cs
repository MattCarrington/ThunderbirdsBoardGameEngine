using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations
{
    public record FindAccessibleLocationsQuery(ThunderbirdCode Thunderbird) : IRequest<FindAccessibleLocationsResponse>;
}
