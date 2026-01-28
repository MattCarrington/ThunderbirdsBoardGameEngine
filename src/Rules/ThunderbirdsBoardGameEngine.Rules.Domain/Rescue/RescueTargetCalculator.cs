namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class RescueTargetCalculator
    {
        public RescueTargetResult CalculateRescueTarget(RescueCalculationInput input, DisasterContribution disasterContribution)
        {
            var appliedBonuses = disasterContribution.ApplyRescueModifier(input);

            var bonus = appliedBonuses.Sum(b => b.Value);

            return new RescueTargetResult
            {
                TargetRoll = disasterContribution.DifficultyNumber - bonus,
                TotalBonus = bonus,
                AppliedBonuses = appliedBonuses.ToList()
            };
        }
    }
}

// public RescueTargetResult Calculate(
//     RescueCalculationContext context,
//     DisasterContribution disaster,
//     IEnumerable<IRescueModifierSource> sources)
// {
//     var appliedModifiers = sources
//         .SelectMany(source => source.GetModifiers(context))
//         .ToList();

//     var totalBonus = appliedModifiers.Sum(m => m.Value);

//     return new RescueTargetResult(
//         targetRoll: disaster.DifficultyNumber - totalBonus,
//         totalBonus: totalBonus,
//         appliedBonuses: appliedModifiers
//     );
// }
