using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces
{
    public interface IMovementClientService
    {
        Task<MovementResultViewModel?> ValidateMovementAsync(string thunderbirdCode, string startLocationCode, string destinationLocationCode, IReadOnlyList<string> eventCards);

        Task<IReadOnlyList<MovementLocationOptions>> GetAccessibleLocationsAsync(string thunderbirdCode);
    }
}