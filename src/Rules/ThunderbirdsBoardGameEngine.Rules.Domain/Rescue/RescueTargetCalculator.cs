namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(RescueCalculationInput input, DisasterContribution disasterContribution)
        {
            var appliedBonuses = disasterContribution.Bonuses
                .Where(b => input.AppliedBonusKeys.Contains(b.Key))
                .ToList();

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new RescueTargetResult
            {
                TargetRoll = disasterContribution.DifficultyNumber - bonus,
                TotalBonus = bonus,
                AppliedBonuses = appliedBonuses
            };
        }
    }
}
