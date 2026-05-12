using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    /// <summary>
    /// Represents a disaster card definition in the reference data.
    /// </summary>
    /// <remarks>
    /// Includes disaster metadata, bonuses, and rewards as loaded from the reference data snapshot.
    /// </remarks>
    public sealed record ReferenceDisasterDefinition
    {
        public CardCode Code { get; }

        public string DisplayName { get; }

        public int DifficultyNumber { get; }

        public LocationCode Location { get; }

        public RescueType RescueType { get; }

        public IReadOnlyList<ReferenceDisasterBonus> Bonuses { get; }

        public IReadOnlyList<ReferenceDisasterReward> Rewards { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDisasterDefinition"/> record.
        /// </summary>
        /// <param name="code">The unique disaster card code.</param>
        /// <param name="displayName">The display name of the disaster.</param>
        /// <param name="difficultyNumber">The difficulty number (must be positive).</param>
        /// <param name="location">The location where the disaster occurs.</param>
        /// <param name="rescueType">The type of rescue required.</param>
        /// <param name="bonuses">The list of bonuses (must not be empty).</param>
        /// <param name="rewards">The list of rewards (must not be empty).</param>
        /// <exception cref="ArgumentException">Thrown when displayName is null or whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when difficultyNumber is zero or negative, or when bonuses or rewards are empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when bonuses or rewards are null.</exception>
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
