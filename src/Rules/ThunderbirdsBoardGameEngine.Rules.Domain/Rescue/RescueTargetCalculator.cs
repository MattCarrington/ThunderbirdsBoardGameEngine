namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(RescueCalculationInput input, DisasterContribution disasterContribution)
        {
            var appliedBonuses = disasterContribution.AvailableBonuses
                .Where(b => input.PresentDisasterBonusKeys.Contains(b.Key))
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
