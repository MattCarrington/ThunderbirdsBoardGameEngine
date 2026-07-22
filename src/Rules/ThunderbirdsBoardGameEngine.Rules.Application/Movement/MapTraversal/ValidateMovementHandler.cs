using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation;

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
            var request = new ValidateMovementInput(
                Thunderbird: query.ThunderbirdCode,
                Start: query.StartLocationCode,
                Destination: query.DestinationLocationCode,
                ActiveEventCards: query.ActiveEventCardCodes
            );

            var movementResult = _validateMovementResolutionService.ResolveMovementValidation(request);

            return Task.FromResult(
                new ValidateMovementResponse(
                    IsValid: movementResult.IsValid,
                    SpacesTravelled: movementResult.SpacesTravelled,
                    Route: movementResult.Route,
                    ActionPointCost: movementResult.ActionPointCost,
                    EffectiveTopSpeed: movementResult.EffectiveTopSpeed,
                    ThunderbirdTopSpeed: movementResult.ThunderbirdTopSpeed,
                    Messages: movementResult.Messages
                )
            );
        }
    }
}
