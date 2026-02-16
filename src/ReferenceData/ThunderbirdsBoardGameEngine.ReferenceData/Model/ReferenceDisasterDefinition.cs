using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceDisasterDefinition(
        CardCode Code,
        string Title,
        int DifficultyNumber,
        LocationCode Location,
        RescueType RescueType,
        IReadOnlyList<ReferenceDisasterBonus> Bonuses,
        IReadOnlyList<ReferenceDisasterReward> Rewards
    );
}
