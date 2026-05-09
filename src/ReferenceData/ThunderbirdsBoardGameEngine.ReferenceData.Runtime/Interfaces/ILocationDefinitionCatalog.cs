using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    public interface ILocationDefinitionCatalog
    {
        ImmutableArray<ReferenceLocationDefinition> GetAll();

        ReferenceLocationDefinition GetByCode(LocationCode code);
    }
}