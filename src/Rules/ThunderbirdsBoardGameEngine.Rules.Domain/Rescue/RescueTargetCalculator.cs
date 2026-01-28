
namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(int difficultyNumber, RescueCalculationInput input, IEnumerable<IBonusModifierSource> sources)
        {
            var appliedBonuses = sources.SelectMany(source => source.ApplyRescueModifier(input)).ToList();

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new RescueTargetResult
            {
                TargetRoll = difficultyNumber - bonus,
                TotalBonus = bonus,
                AppliedBonuses = appliedBonuses.ToList()
            };
        }
    }
}