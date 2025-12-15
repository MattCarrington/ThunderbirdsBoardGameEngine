namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class BonusCalculator
    {
        public BonusCalculationResult CalculateRescueTarget(IReadOnlyCollection<string> appliedBonusKeys, RescueContext rescueContext)
        {
            var appliedBonuses = rescueContext.Bonuses
                .Where(b => appliedBonusKeys.Contains(b.Key))
                .ToList();

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new BonusCalculationResult
            {
                TargetRoll = rescueContext.DifficultyNumber - bonus,
                TotalBonus = bonus,
                AppliedBonuses = appliedBonuses
            };
        }            
    }
}
