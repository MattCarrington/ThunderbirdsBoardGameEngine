using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal
{
    public class ValidateMovementHandler : IRequestHandler<ValidateMovementQuery, ValidateMovementResponse>
    {
        private readonly IValidateMovementResolutionService _validateMovementResolutionService;

        public ValidateMovementHandler(IValidateMovementResolutionService validateMovementResolutionService)
        {
            _validateMovementResolutionService = validateMovementResolutionService;
        }

        public Task<ValidateMovementResponse> Handle(ValidateMovementQuery query, CancellationToken cancellationToken)
        {
            var request = new MovementRequest(
                Thunderbird: query.Thunderbird,
                Start: query.Start,
                Destination: query.Destination
            );

            var movementResult = _validateMovementResolutionService.ResolveMovementValidation(request);

            return Task.FromResult(
                new ValidateMovementResponse(
                    IsValid: movementResult.IsValid,
                    SpacesTravelled: movementResult.SpacesTravelled,
                    Route: movementResult.Route,
                    ActionPointCost: movementResult.ActionPointCost,
                    TopSpeed: movementResult.TopSpeed,
                    Messages: movementResult.Messages
                )
            );
        }
    }
}
