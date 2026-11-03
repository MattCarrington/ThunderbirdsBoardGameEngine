using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public interface IMovementRulesGateway
    {
        Task<ValidateMovementDecision> ValidateMovementAsync(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken);
    }
}
