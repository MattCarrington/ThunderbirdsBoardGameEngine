using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed class DisasterContribution : IBonusModifierSource
    {
        public int DifficultyNumber { get; }

        public IReadOnlyList<DisasterBonus> AvailableBonuses { get; }

        public RescueType RescueType { get; }

        public DisasterContribution(int difficultyNumber, IReadOnlyList<DisasterBonus> availableBonuses, RescueType rescueType)
        {
            DifficultyNumber = difficultyNumber;
            AvailableBonuses = availableBonuses;
            RescueType = rescueType;

        }

        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            return AvailableBonuses
                .Where(b => input.PresentDisasterBonusKeys.Contains(b.Key))
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