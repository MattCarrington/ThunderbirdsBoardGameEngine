using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Validators;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementResolutionService : IValidateMovementResolutionService
    {
        private readonly IThunderbirdsDefinitionLookup _thunderbirdsDefinitionLookup;
        private readonly ILocationDefinitionLookup _locationDefinitionLookup;
        private readonly IMapEdgeDefinitionLookup _edgeDefinitionLookup;
        private readonly IEventCardValidator _eventCardValidator;
        private readonly MovementEvaluator _movementEvaluator;

        public ValidateMovementResolutionService(IThunderbirdsDefinitionLookup thunderbirdsDefinitionLookup,
            ILocationDefinitionLookup locationDefinitionLookup,
            IMapEdgeDefinitionLookup edgeDefinitionLookup,
            IEventCardValidator eventCardValidator,
            MovementEvaluator movementEvaluator)
        {
            _thunderbirdsDefinitionLookup = thunderbirdsDefinitionLookup ?? throw new ArgumentNullException(nameof(thunderbirdsDefinitionLookup));
            _locationDefinitionLookup = locationDefinitionLookup ?? throw new ArgumentNullException(nameof(locationDefinitionLookup));
            _edgeDefinitionLookup = edgeDefinitionLookup ?? throw new ArgumentNullException(nameof(edgeDefinitionLookup));
            _eventCardValidator = eventCardValidator ?? throw new ArgumentNullException(nameof(eventCardValidator));
            _movementEvaluator = movementEvaluator ?? throw new ArgumentNullException(nameof(movementEvaluator));
        }

        public ValidateMovementResult ResolveMovementValidation(ValidateMovementInput request)
        {
            var thunderbird = _thunderbirdsDefinitionLookup.GetThunderbirdMovementContribution(request.Thunderbird);

            if (!_locationDefinitionLookup.Exists(request.Start))
            {
                throw new ReferenceDataNotFoundException(
                    resourceType: "Location",
                    code: request.Start.Value);
            }

            if (!_locationDefinitionLookup.Exists(request.Destination))
            {
                throw new ReferenceDataNotFoundException(
                    resourceType: "Location",
                    code: request.Destination.Value);
            }

            var edges = _edgeDefinitionLookup.GetAll();

            var topography = new Topography(edges);

            _eventCardValidator.Validate(request.ActiveEventCards);

            var input = new MovementEvaluationInput(thunderbird, topography, request.Start, request.Destination, request.ActiveEventCards);

            var evaluationResult = _movementEvaluator.Evaluate(input);

            return new ValidateMovementResult(
                IsValid: evaluationResult.IsMoveValid,
                SpacesTravelled: evaluationResult.SpacesTravelled,
                Route: evaluationResult.Route,
                ActionPointCost: evaluationResult.ActionPointCost,
                EffectiveTopSpeed: evaluationResult.TopSpeed,
                ThunderbirdTopSpeed: thunderbird.TopSpeed,
                Messages: evaluationResult.Messages);
        }
    }
}
