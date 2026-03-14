using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceCharacterDefinition(
        CharacterCode Code,
        string DisplayName,
        ReferenceCharacterRescueBonus? RescueBonus
    );
}
