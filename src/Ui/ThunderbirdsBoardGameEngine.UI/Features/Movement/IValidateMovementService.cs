using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public interface IValidateMovementService
    {
        Task<ValidateMovementResponseDto?> ValidateMovementAsync(string thunderbirdCode, string startLocationCode, string destinationLocationCode);
    }
}