using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IEventCardCatalogLookup
    {
        bool Exists(CardCode cardCode);
    }
}