using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    internal interface IDisasterCardMapper
    {
        DisasterCard Map(DisasterCardCatalogDto dto);
    }
}
