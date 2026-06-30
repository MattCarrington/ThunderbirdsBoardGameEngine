using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public interface IValidateMovementGateway
    {
        Task<bool> ValidateMovement(ThunderbirdCode thunderbirdCode, LocationCode startLocation, LocationCode destination, CancellationToken cancellationToken);
    }
}
