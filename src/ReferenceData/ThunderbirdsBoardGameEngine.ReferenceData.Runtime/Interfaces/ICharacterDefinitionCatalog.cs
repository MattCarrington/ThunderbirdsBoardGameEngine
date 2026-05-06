using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    public interface ICharacterDefinitionCatalog
    {
        ImmutableArray<ReferenceCharacterDefinition> GetAll();
        ReferenceCharacterDefinition GetByCode(CharacterCode code);
    }
}