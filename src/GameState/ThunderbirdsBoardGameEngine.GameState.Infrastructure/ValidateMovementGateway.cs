using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.MapTraversal;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    internal class ValidateMovementGateway : IValidateMovementGateway
    {
        private readonly IMediator _mediator;

        public ValidateMovementGateway(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<bool> ValidateMovement(ThunderbirdCode thunderbirdCode, LocationCode startLocation, LocationCode destination, CancellationToken cancellationToken)
        {
            var request = new ValidateMovementQuery(thunderbirdCode, startLocation, destination);
            var response = await _mediator.Send(request, cancellationToken);

            return response.IsValid;
        }
    }
}
