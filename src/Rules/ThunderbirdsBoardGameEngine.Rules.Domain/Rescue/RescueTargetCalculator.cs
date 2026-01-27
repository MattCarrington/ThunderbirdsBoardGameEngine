using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(RescueCalculationInput input, DisasterContribution disasterContribution)
        {
            var appliedBonuses = ApplyDisasterBonuses(input.PresentDisasterBonusKeys, disasterContribution.AvailableBonuses);

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new RescueTargetResult
            {
                TargetRoll = disasterContribution.DifficultyNumber - bonus,
                TotalBonus = bonus,
                AppliedBonuses = appliedBonuses.ToList()
            };
        }

        private static IReadOnlyCollection<AppliedRescueModifier> ApplyDisasterBonuses(
            IReadOnlyCollection<DisasterBonusKey> presentDisasterBonusKeys,
            IReadOnlyCollection<DisasterBonus> availableBonuses)
        {
            return availableBonuses
                .Where(b => presentDisasterBonusKeys.Contains(b.Key))
                .Select(b => new AppliedRescueModifier
                {
                    SourceType = "disaster-card",
                    Key = b.Key.Value,
                    Value = b.Value
                })
                .ToList();
        }
    }
}
