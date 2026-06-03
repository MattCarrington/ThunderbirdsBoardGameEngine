using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementHandler : IRequestHandler<ValidateMovementQuery, ValidateMovementResponse>
    {
        private readonly IThunderbirdsDefinitionLookup _thunderbirdsDefinitionLookup;
        private readonly ILocationDefinitionLookup _locationDefinitionLookup;
        private readonly IMapEdgeDefinitionLookup _edgeDefinitionLookup;
        private readonly MovementValidator _movementValidator;
        private readonly BreadthFirstRouteFinder _breadthFirstRouteFinder;
        private readonly ActionPointCalculator _actionPointCalculator;

        public ValidateMovementHandler(
            IThunderbirdsDefinitionLookup thunderbirdsDefinitionLookup,
            ILocationDefinitionLookup locationDefinitionLookup,
            IMapEdgeDefinitionLookup edgeDefinitionLookup,
            MovementValidator movementValidator,
            BreadthFirstRouteFinder breadthFirstRouteFinder,
            ActionPointCalculator actionPointCalculator)
        {
            _thunderbirdsDefinitionLookup = thunderbirdsDefinitionLookup;
            _locationDefinitionLookup = locationDefinitionLookup;
            _edgeDefinitionLookup = edgeDefinitionLookup;
            _movementValidator = movementValidator;
            _breadthFirstRouteFinder = breadthFirstRouteFinder;
            _actionPointCalculator = actionPointCalculator;
        }

        public Task<ValidateMovementResponse> Handle(ValidateMovementQuery request, CancellationToken cancellationToken)
        {
            var thunderbird = _thunderbirdsDefinitionLookup.GetThunderbirdMovementContribution(request.Thunderbird);

            var locations = _locationDefinitionLookup.GetAllLocationContributions();
            var edges = _edgeDefinitionLookup.GetAll();

            var topography = new Topography(locations, edges);

            var movementRequest = new MovementRequest(thunderbird, topography, request.Start, request.Destination);

            var validationResult = _movementValidator.Validate(movementRequest);

            if (!validationResult.IsValid)
            {
                throw new InvalidMovementCalculationRequestException(validationResult.ErrorCode!, validationResult.ErrorMessage!);
            }

            var result = _breadthFirstRouteFinder.FindShortestRoute(movementRequest);

            if (result is null)
            {
                return Task.FromResult(new ValidateMovementResponse(
                    IsValid: false,
                    SpacesTravelled: 0,
                    Route: Array.Empty<LocationCode>(),
                    ActionPointCost: 0,
                    TopSpeed: 0,
                    Messages: new[] { $"No route found from {request.Start.Value} to {request.Destination.Value} for {request.Thunderbird}" }));
            }

            var actionPointCost = _actionPointCalculator.CalculateActionPoints(result.SpacesTravelled, thunderbird.TopSpeed);

            return Task.FromResult(new ValidateMovementResponse(
                IsValid: true,
                SpacesTravelled: result.SpacesTravelled,
                Route: result.Route,
                ActionPointCost: actionPointCost,
                TopSpeed: thunderbird.TopSpeed,
                Messages: Array.Empty<string>()));
        }
    }
}
