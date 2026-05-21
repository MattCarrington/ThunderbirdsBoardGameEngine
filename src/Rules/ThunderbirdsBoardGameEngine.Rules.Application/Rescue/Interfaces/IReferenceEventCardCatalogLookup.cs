using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IReferenceEventCardCatalogLookup
    {
        bool Exists(CardCode cardCode);
    }
}