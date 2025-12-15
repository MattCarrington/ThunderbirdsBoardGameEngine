namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class BonusCalculator
    {
        public BonusCalculationResult CalculateRescueTarget(int difficultyNumber, IReadOnlyCollection<string> appliedBonusKeys, IReadOnlyCollection<RuleBonus> cardBonuses)
        {
            var appliedBonuses = cardBonuses
                .Where(b => appliedBonusKeys.Contains(b.Key))
                .ToList();

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new BonusCalculationResult
            {
                TargetRoll = difficultyNumber - bonus,
                TotalBonus = bonus
            };
        }            
    }
}
