using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.AccessibleLocations
{
    public sealed class FindAccessibleLocationsHandler : IRequestHandler<FindAccessibleLocationsQuery, FindAccessibleLocationsResponse>
    {
        private readonly IThunderbirdsDefinitionLookup _thunderbirdsDefinitionLookup;
        private readonly IMapEdgeDefinitionLookup _mapEdgeDefinitionLookup;

        public FindAccessibleLocationsHandler(IThunderbirdsDefinitionLookup thunderbirdsDefinitionLookup, IMapEdgeDefinitionLookup mapEdgeDefinitionLookup)
        {
            _thunderbirdsDefinitionLookup = thunderbirdsDefinitionLookup ?? throw new ArgumentNullException(nameof(thunderbirdsDefinitionLookup));
            _mapEdgeDefinitionLookup = mapEdgeDefinitionLookup ?? throw new ArgumentNullException(nameof(mapEdgeDefinitionLookup));
        }

        public Task<FindAccessibleLocationsResponse> Handle(FindAccessibleLocationsQuery request, CancellationToken cancellationToken)
        {
            var thunderbird = _thunderbirdsDefinitionLookup.GetThunderbirdMovementContribution(request.Thunderbird);

            var topography = new Topography(_mapEdgeDefinitionLookup.GetAll());

            var result = topography.GetAccessibleLocationsForDomain(thunderbird.TraversalDomain);

            return Task.FromResult(new FindAccessibleLocationsResponse(result));
        }
    }
}
