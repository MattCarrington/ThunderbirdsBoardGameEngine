using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface IDisasterCardDeserializer
    {
        IReadOnlyList<DisasterCardCatalogDto> Deserialize(JsonElement data);
    }
}