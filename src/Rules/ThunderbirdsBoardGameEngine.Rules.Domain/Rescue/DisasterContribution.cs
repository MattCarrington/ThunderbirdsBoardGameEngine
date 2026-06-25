using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    /// <summary>
    /// Represents a source of rescue modifiers contributed by a disaster card, including its difficulty, available
    /// bonuses, and associated rescue type.
    /// </summary>
    /// <remarks>This class encapsulates the disaster-specific bonuses that can be applied during a rescue
    /// calculation. It is typically used as part of the rescue modifier calculation process, where each available bonus
    /// is evaluated based on the current rescue context.</remarks>
    public sealed class DisasterContribution : IRescueModifierSource
    {
        public int DifficultyNumber { get; }

        public IReadOnlyList<DisasterBonus> AvailableBonuses { get; }

        public RescueType RescueType { get; }

        public DisasterContribution(int difficultyNumber, IReadOnlyList<DisasterBonus> availableBonuses, RescueType rescueType)
        {
            DifficultyNumber = difficultyNumber;
            AvailableBonuses = availableBonuses;
            RescueType = rescueType;

        }

        /// <summary>
        /// Applies disaster card rescue bonuses based on presence of bonuses printed on the card.
        /// </summary>
        /// <remarks>
        /// The disaster card is the authoritative source of applicable bonuses.
        /// The input may contain irrelevant, unknown, or duplicate bonus keys; these are ignored by design.
        /// Each printed disaster bonus may contribute at most once.
        /// Rescue type and board state are not considered by this component.
        /// </remarks>
        /// <param name="input">
        /// Shared rescue calculation context. Must not be null.
        /// </param>
        /// <returns>
        /// Zero or more applied rescue modifiers corresponding to bonuses printed on the disaster card.
        /// </returns>
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            foreach (var bonus in AvailableBonuses)
            {
                if (input.PresentDisasterBonusKeys.Contains(bonus.Key))
                {
                    yield return new AppliedRescueModifier
                    {
                        SourceType = SourceType.DisasterCard,
                        Key = bonus.Key.Value,
                        Value = bonus.Value
                    };
                }
            }
        }
    }
}