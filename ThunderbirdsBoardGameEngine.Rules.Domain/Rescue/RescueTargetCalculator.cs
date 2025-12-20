namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(IReadOnlyCollection<string> appliedBonusKeys, DisasterContribution disasterContribution)
        {
            var appliedBonuses = disasterContribution.Bonuses
                .Where(b => appliedBonusKeys.Contains(b.Key))
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
