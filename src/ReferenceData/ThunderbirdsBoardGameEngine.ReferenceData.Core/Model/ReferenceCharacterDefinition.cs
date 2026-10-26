using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceCharacterDefinition
    {
        public CharacterCode Code { get; }

        public string DisplayName { get; }

        public ReferenceCharacterRescueBonus? RescueBonus { get; }

        [JsonConstructor]
        internal ReferenceCharacterDefinition(
            CharacterCode code,
            string displayName,
            ReferenceCharacterRescueBonus? rescueBonus)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
            RescueBonus = rescueBonus;
        }
    }
}
