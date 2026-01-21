using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface ICharacterDefinitionDeserializer
    {
        IReadOnlyList<CharacterCatalogDto> Deserialize(JsonElement data);
    }
}