using ThunderbirdsBoardGameEngine.GameState.Domain.Movement;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application
{
    public interface IThunderbirdMovementGateway
    {
        Task<ValidateMovementDecision> ValidateMovement(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken);
    }
}
