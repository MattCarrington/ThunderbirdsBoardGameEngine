using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IFabCardCatalogLookup
    {
        bool Exists(CardCode cardCode);
    }
}