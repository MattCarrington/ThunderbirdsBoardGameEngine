using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceCharacterDefinition
    {
        public CharacterCode Code { get; }

        public string DisplayName { get; }

        public ReferenceCharacterRescueBonus? RescueBonus { get; }

        public ReferenceCharacterDefinition(
            CharacterCode code,
            string displayName,
            ReferenceCharacterRescueBonus? rescueBonus)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
            RescueBonus = rescueBonus;
        }
    }
}
