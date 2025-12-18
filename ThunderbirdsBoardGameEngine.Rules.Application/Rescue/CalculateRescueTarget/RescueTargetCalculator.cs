namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(IReadOnlyCollection<string> appliedBonusKeys, RescueProjection rescueContext)
        {
            var appliedBonuses = rescueContext.Bonuses
                .Where(b => appliedBonusKeys.Contains(b.Key))
                .ToList();

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new RescueTargetResult
            {
                TargetRoll = rescueContext.DifficultyNumber - bonus,
                TotalBonus = bonus,
                AppliedBonuses = appliedBonuses
            };
        }            
    }
}
