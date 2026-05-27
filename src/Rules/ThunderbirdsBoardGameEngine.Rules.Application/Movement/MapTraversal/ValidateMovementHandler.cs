using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementHandler : IRequestHandler<ValidateMovementQuery, ValidateMovementResponse>
    {
        private readonly IThunderbirdsDefinitionLookup _thunderbirdsDefinitionLookup;
        private readonly ILocationDefinitionLookup _locationDefinitionLookup;
        private readonly IMapEdgeDefinitionLookup _edgeDefinitionLookup;
        private readonly BreadthFirstRouteFinder _breadthFirstRouteFinder;

        public ValidateMovementHandler(
            IThunderbirdsDefinitionLookup thunderbirdsDefinitionLookup,
            ILocationDefinitionLookup locationDefinitionLookup,
            IMapEdgeDefinitionLookup edgeDefinitionLookup,
            BreadthFirstRouteFinder breadthFirstRouteFinder)
        {
            _thunderbirdsDefinitionLookup = thunderbirdsDefinitionLookup;
            _locationDefinitionLookup = locationDefinitionLookup;
            _edgeDefinitionLookup = edgeDefinitionLookup;
            _breadthFirstRouteFinder = breadthFirstRouteFinder;
        }

        public Task<ValidateMovementResponse> Handle(ValidateMovementQuery request, CancellationToken cancellationToken)
        {
            var thunderbird = _thunderbirdsDefinitionLookup.GetThunderbirdMovementContribution(request.Thunderbird);

            var locations = _locationDefinitionLookup.GetAll();
            var edges = _edgeDefinitionLookup.GetAll();

            if (!locations.Any(l => l.Code == request.Start))
            {
                throw new InvalidMovementCalculationRequestException("Location", request.Start.Value);
            }

            if (!locations.Any(l => l.Code == request.Destination))
            {
                throw new InvalidMovementCalculationRequestException("Location", request.Destination.Value);
            }

            var topography = new Topography(locations, edges);

            var movementRequest = new MovementRequest(thunderbird, topography, request.Start, request.Destination);

            var result = _breadthFirstRouteFinder.FindShortestRoute(movementRequest)
                ?? throw new InvalidMovementCalculationRequestException("Route", $"No route found from {request.Start.Value} to {request.Destination.Value} for {request.Thunderbird}");

            return Task.FromResult(new ValidateMovementResponse(result.SpacesTravelled));
        }
    }
}
