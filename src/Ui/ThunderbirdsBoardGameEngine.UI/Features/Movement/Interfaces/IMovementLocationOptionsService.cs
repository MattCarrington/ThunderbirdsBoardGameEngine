using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces
{
    public interface IMovementLocationOptionsService
    {
        IReadOnlyList<MovementLocationOptions> GetAll();
    }
}