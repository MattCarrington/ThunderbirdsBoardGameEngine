using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models
{
    public sealed record DisasterBonusKeyDefinition(
        DisasterBonusKey Key,
        string DisplayName
    );
}