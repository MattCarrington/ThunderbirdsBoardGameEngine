using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    public interface IDisasterBonusKeyDefinitionCatalog
    {
        DisasterBonusKeyDefinition GetByCode(DisasterBonusKey key);
    }
}