using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceDisasterDefinition
    {
        public CardCode Code { get; }

        public string DisplayName { get; }

        public int DifficultyNumber { get; }

        public LocationCode Location { get; }

        public RescueType RescueType { get; }

        public IReadOnlyList<ReferenceDisasterBonus> Bonuses { get; }

        public IReadOnlyList<ReferenceDisasterReward> Rewards { get; }

        public ReferenceDisasterDefinition(
            CardCode code,
            string displayName,
            int difficultyNumber,
            LocationCode location,
            RescueType rescueType,
            IReadOnlyList<ReferenceDisasterBonus> bonuses,
            IReadOnlyList<ReferenceDisasterReward> rewards)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(difficultyNumber);
            ArgumentNullException.ThrowIfNull(bonuses);
            ArgumentNullException.ThrowIfNull(rewards);
            ArgumentOutOfRangeException.ThrowIfZero(bonuses.Count);
            ArgumentOutOfRangeException.ThrowIfZero(rewards.Count);

            Code = code;
            DisplayName = displayName;
            DifficultyNumber = difficultyNumber;
            Location = location;
            RescueType = rescueType;
            Bonuses = bonuses;
            Rewards = rewards;
        }
    }
}
