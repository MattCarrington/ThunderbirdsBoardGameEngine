using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.MoveThunderbird
{
    public sealed class FakeMovementRulesGateway : IMovementRulesGateway
    {
        private readonly ValidateMovementDecision _decision;

        public FakeMovementRulesGateway(ValidateMovementDecision decision)
        {
            _decision = decision;
        }

        public Task<ValidateMovementDecision> ValidateMovementAsync(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken)
        {
            return Task.FromResult(_decision);
        }
    }
}
