using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.MoveThunderbird
{
    public sealed class FakeValidateMovementGateway : IValidateMovementGateway
    {
        private readonly ValidateMovementDecision _decision;

        public FakeValidateMovementGateway(ValidateMovementDecision decision)
        {
            _decision = decision;
        }

        public Task<ValidateMovementDecision> ValidateMovement(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken)
        {
            return Task.FromResult(_decision);
        }
    }
}
