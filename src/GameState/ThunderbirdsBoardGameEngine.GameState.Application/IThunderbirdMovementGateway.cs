using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application
{
    public interface IThunderbirdMovementGateway
    {
        bool IsValid(ThunderbirdCode thunderbird, LocationCode origin, LocationCode destination, CancellationToken cancellationToken);
    }
}
