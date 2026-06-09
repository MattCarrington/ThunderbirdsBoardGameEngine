namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public interface IMovementLocationOptionsService
    {
        IReadOnlyList<MovementLocationOptions> GetAll();
    }
}