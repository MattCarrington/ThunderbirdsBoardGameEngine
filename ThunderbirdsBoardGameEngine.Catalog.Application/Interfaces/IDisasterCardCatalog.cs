using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface IDisasterCardCatalog
    {
        string Version { get; }

        IReadOnlyList<DisasterCard> All { get; }
    }
}
