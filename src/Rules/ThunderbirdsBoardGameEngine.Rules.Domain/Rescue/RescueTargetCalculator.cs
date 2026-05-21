
namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    /// <summary>
    /// Provides functionality to calculate the rescue target value based on difficulty, input parameters, and
    /// applicable bonus modifiers.
    /// </summary>
    /// <remarks>This class is typically used in scenarios where a rescue roll or check is required, and the
    /// final target value must account for various dynamic modifiers. The calculation aggregates all bonuses from the
    /// provided sources to determine the adjusted target.</remarks>
    public class RescueTargetCalculator
    {
        /// <summary>
        /// Calculates the rescue target result by applying all relevant bonus modifiers to the specified difficulty
        /// number.
        /// </summary>
        /// <remarks>All bonus modifiers from the provided sources are aggregated and subtracted from the
        /// base difficulty number to determine the final target roll. The method does not modify the input parameters
        /// or the sources.</remarks>
        /// <param name="difficultyNumber">The base difficulty number to which bonus modifiers will be applied.</param>
        /// <param name="input">The input parameters used for rescue calculation, providing context for bonus modifiers.</param>
        /// <param name="sources">A collection of sources that provide bonus modifiers to be applied to the rescue calculation.</param>
        /// <returns>A RescueTargetResult containing the adjusted target roll, the total bonus applied, and details of all
        /// applied bonuses.</returns>
        public RescueTargetResult CalculateRescueTarget(int difficultyNumber, RescueCalculationInput input, IEnumerable<IRescueModifierSource> sources)
        {
            var appliedBonuses = sources.SelectMany(source => source.ApplyRescueModifier(input)).ToList();

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new RescueTargetResult
            {
                TargetRoll = Math.Max(0, difficultyNumber - bonus),
                TotalBonus = bonus,
                AppliedModifiers = appliedBonuses
            };
        }
    }
}