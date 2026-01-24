using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface ICharacterDefinitionReferenceSource
    {
        string Version { get; }

        ImmutableArray<CharacterDefinition> Characters { get; }

        CharacterDefinition GetCharacterDefinition(Character character);
    }
}
