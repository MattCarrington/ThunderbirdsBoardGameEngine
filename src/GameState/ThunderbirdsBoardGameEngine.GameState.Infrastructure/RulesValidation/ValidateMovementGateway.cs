using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.RulesValidation
{
    internal sealed class ValidateMovementGateway : IValidateMovementGateway
    {
        private readonly IMediator _mediator;

        public ValidateMovementGateway(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ValidateMovementDecision> ValidateMovement(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken)
        {
            var validateMovementQuery = new ValidateMovementQuery(thunderbird, origin, destination, Array.Empty<CardCode>());

            var result = await _mediator.Send(validateMovementQuery, cancellationToken);

            return new ValidateMovementDecision(result.IsValid, result.Messages);
        }
    }
}
