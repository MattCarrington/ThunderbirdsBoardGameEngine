using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain
{
    public interface ICardSource
    {
        CardCode CardCode { get; }
    }
}
