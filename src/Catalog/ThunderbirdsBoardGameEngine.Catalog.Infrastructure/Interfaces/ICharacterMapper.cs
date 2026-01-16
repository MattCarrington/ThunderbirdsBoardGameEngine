using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces
{
    internal interface ICharacterMapper
    {
        CharacterDefinition Map(CharacterCatalogDto dto);
    }
}
