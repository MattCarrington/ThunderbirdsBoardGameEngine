using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    public interface IDisasterDefinitionCatalog
    {
        ImmutableArray<ReferenceDisasterDefinition> GetAll();
        ReferenceDisasterDefinition GetByCode(CardCode code);
    }
}