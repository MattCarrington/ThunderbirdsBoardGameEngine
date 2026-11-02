using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public interface IValidateMovementGateway
    {
        Task<ValidateMovementDecision> ValidateMovement(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken);
    }
}
