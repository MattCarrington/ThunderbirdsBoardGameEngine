using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface ICharacterDefinitionService
    {
        ImmutableArray<CharacterDefinition> GetAll();
    }
}