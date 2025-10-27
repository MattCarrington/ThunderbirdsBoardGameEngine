using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface IDisasterCardCatalog
    {
        string Version { get; }

        ImmutableArray<DisasterCard> All { get; }
    }
}
