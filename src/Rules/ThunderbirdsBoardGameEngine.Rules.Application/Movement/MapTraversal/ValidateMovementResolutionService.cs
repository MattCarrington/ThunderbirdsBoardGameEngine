using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementResolutionService : IValidateMovementResolutionService
    {
        private readonly IThunderbirdsDefinitionLookup _thunderbirdsDefinitionLookup;
        private readonly ILocationDefinitionLookup _locationDefinitionLookup;
        private readonly IMapEdgeDefinitionLookup _edgeDefinitionLookup;
        private readonly MovementValidator _movementValidator;
        private readonly BreadthFirstRouteFinder _breadthFirstRouteFinder;
        private readonly ActionPointCalculator _actionPointCalculator;

        public ValidateMovementResolutionService(IThunderbirdsDefinitionLookup thunderbirdsDefinitionLookup,
            ILocationDefinitionLookup locationDefinitionLookup,
            IMapEdgeDefinitionLookup edgeDefinitionLookup,
            MovementValidator movementValidator,
            BreadthFirstRouteFinder breadthFirstRouteFinder,
            ActionPointCalculator actionPointCalculator)
        {
            _thunderbirdsDefinitionLookup = thunderbirdsDefinitionLookup ?? throw new ArgumentNullException(nameof(thunderbirdsDefinitionLookup));
            _locationDefinitionLookup = locationDefinitionLookup ?? throw new ArgumentNullException(nameof(locationDefinitionLookup));
            _edgeDefinitionLookup = edgeDefinitionLookup ?? throw new ArgumentNullException(nameof(edgeDefinitionLookup));
            _movementValidator = movementValidator ?? throw new ArgumentNullException(nameof(movementValidator));
            _breadthFirstRouteFinder = breadthFirstRouteFinder ?? throw new ArgumentNullException(nameof(breadthFirstRouteFinder));
            _actionPointCalculator = actionPointCalculator ?? throw new ArgumentNullException(nameof(actionPointCalculator));
        }

        public MovementResponse ResolveMovementValidation(MovementRequest request)
        {
            var thunderbird = _thunderbirdsDefinitionLookup.GetThunderbirdMovementContribution(request.Thunderbird);

            var locations = _locationDefinitionLookup.GetAllLocationContributions();
            var edges = _edgeDefinitionLookup.GetAll();

            var topography = new Topography(locations, edges);

            var movementRequest = new MovementInput(thunderbird, topography, request.Start, request.Destination);

            var validationResult = _movementValidator.Validate(movementRequest);

            if (!validationResult.IsValid)
            {
                throw new InvalidMovementCalculationRequestException(validationResult.ErrorCode!, validationResult.ErrorMessage!);
            }

            var routeResult = _breadthFirstRouteFinder.FindShortestRoute(movementRequest);

            if (routeResult is null)
            {
                return new MovementResponse(
                    IsValid: false,
                    SpacesTravelled: 0,
                    Route: Array.Empty<LocationCode>(),
                    ActionPointCost: 0,
                    TopSpeed: 0,
                    Messages: new[] { $"No route found from {request.Start.Value} to {request.Destination.Value} for {request.Thunderbird}" });
            }

            var actionPointCost = _actionPointCalculator.CalculateActionPoints(routeResult.SpacesTravelled, thunderbird.TopSpeed);

            return new MovementResponse(
                IsValid: true,
                SpacesTravelled: routeResult.SpacesTravelled,
                Route: routeResult.Route,
                ActionPointCost: actionPointCost,
                TopSpeed: thunderbird.TopSpeed,
                Messages: Array.Empty<string>());
        }
    }
}
