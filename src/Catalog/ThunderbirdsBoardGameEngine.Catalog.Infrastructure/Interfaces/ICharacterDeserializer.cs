using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface ICharacterDeserializer
    {
        IReadOnlyList<CharacterCatalogDto> Deserialize(JsonElement data);
    }
}