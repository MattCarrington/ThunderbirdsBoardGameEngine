using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface ICharacterDefinitionReferenceSource
    {
        string Version { get; }

        ImmutableArray<CharacterDefinition> Characters { get; }
    }
}
