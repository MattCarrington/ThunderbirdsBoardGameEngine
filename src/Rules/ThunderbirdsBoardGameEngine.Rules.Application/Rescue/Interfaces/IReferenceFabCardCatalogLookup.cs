using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IReferenceFabCardCatalogLookup
    {
        bool Exists(CardCode cardCode);
    }
}