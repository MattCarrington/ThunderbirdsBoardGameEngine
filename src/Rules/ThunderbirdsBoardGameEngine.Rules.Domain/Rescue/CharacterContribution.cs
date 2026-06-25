

using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    /// <summary>
    /// Provides rescue modifiers based on the acting character's special ability.
    /// </summary>
    /// <remarks>
    /// The character contribution represents the bonus granted by the performing character,
    /// if they have a rescue ability applicable to the current rescue type.    
    /// </remarks>
    public sealed class CharacterContribution : IRescueModifierSource
    {
        public CharacterCode Key { get; }

        public CharacterRescueBonusContribution? RescueBonusContribution { get; }

        /// <summary>
        /// Initializes a new instance of the CharacterContribution class with the specified character code and optional
        /// rescue bonus contribution.
        /// </summary>
        /// <param name="key">The character code that identifies the character for this contribution.</param>
        /// <param name="characterRescueBonusContribution">An optional value representing the rescue bonus contribution for the character. Specify null if no bonus
        /// contribution applies.</param>
        public CharacterContribution(CharacterCode key, CharacterRescueBonusContribution? characterRescueBonusContribution)
        {
            Key = key;
            RescueBonusContribution = characterRescueBonusContribution;
        }

        /// <summary>
        /// Applies the rescue modifier to the specified input and returns any resulting modifiers.
        /// </summary>
        /// <param name="input">The input data containing rescue calculation parameters. The rescue type in this input determines whether a
        /// modifier is applied.</param>
        /// <returns>An enumerable collection of applied rescue modifiers. The collection will contain a single modifier if the
        /// rescue type matches; otherwise, it will be empty.</returns>
        /// <remarks> Characters without a rescue ability (e.g. Lady Penelope) or whose ability does not
        /// match the rescue type contribute no modifiers.</remarks>
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (RescueBonusContribution is null)
            {
                yield break;
            }

            if (RescueBonusContribution.RescueType == input.RescueType)
            {
                yield return new AppliedRescueModifier
                {
                    SourceType = SourceType.CharacterAbility,
                    Key = Key.ToString(),
                    Value = RescueBonusContribution.BonusValue
                };
            }
        }
    }
}
