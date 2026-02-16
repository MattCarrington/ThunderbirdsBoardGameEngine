using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceDisasterBonus(
        DisasterBonusKey Key,
        int Value,
        LocationCode? Location);
}
