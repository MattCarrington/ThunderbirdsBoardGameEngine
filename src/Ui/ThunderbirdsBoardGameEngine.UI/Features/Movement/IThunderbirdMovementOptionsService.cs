namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public interface IThunderbirdMovementOptionsService
    {
        IReadOnlyList<ThunderbirdMovementOptions> GetAllMobileVehicles();
    }
}