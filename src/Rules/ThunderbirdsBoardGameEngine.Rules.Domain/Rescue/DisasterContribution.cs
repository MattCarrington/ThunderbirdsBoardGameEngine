using System;
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
            foreach (var bonus in AvailableBonuses)
            {
                if (input.PresentDisasterBonusKeys.Contains(bonus.Key))
                {
                    yield return new AppliedRescueModifier
                    {
                        SourceType = SourceType.DisasterCard,
                        Key = bonus.Key.Value,
                        Value = bonus.Value
                    };
                }
            }
        }
    }
}