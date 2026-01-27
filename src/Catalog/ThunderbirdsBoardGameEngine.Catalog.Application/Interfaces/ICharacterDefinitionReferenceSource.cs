using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Represents a read-only view of the character reference source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface exposes character definitions for inspection and querying
    /// without allowing modification.
    /// </para>
    /// </remarks>
    public interface ICharacterDefinitionReferenceSource
    {
        string Version { get; }

        ImmutableArray<CharacterDefinition> Characters { get; }

        CharacterDefinition GetCharacterDefinition(Character character);
    }
}
