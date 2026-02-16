using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceCharacterDefinition(
        CharacterCode Code,
        string DisplayName,
        ReferenceCharacterRescueBonus? RescueBonus
    );
}
