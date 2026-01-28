using ThunderbirdsBoardGameEngine.Catalog.Application.Factories;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    public sealed class CatalogDisasterContributionLookup : IDisasterContributionLookup
    {
        private readonly IDisasterCardReferenceSource _disasterCardReferenceSource;

        public CatalogDisasterContributionLookup(IDisasterCardReferenceSource disasterCardReferenceSource)
        {
            _disasterCardReferenceSource = disasterCardReferenceSource ?? throw new ArgumentNullException(nameof(disasterCardReferenceSource));
        }

        public DisasterContribution GetDisasterContribution(CardCode disasterCode)
        {
            var disasterCard = _disasterCardReferenceSource.GetByCode(disasterCode);

            return new DisasterContribution
            (
                difficultyNumber: disasterCard.DifficultyNumber,
                availableBonuses: disasterCard.BonusConditions.Select(ProjectBonus).ToList(),
                rescueType: disasterCard.RescueType
            );
        }

        private static DisasterBonus ProjectBonus(BonusCondition condition)
        {
            return condition switch
            {
                CharacterBonusCondition c =>
                    new DisasterBonus(
                        Key: DisasterBonusKeyFactory.ForCharacter(c.Character),
                        Value: c.BonusValue),

                ThunderbirdBonusCondition t =>
                    new DisasterBonus(
                        Key: DisasterBonusKeyFactory.ForThunderbird(t.Thunderbird),
                        Value: t.BonusValue),

                PodVehicleBonusCondition p =>
                    new DisasterBonus(
                        Key: DisasterBonusKeyFactory.ForPodVehicle(p.PodVehicle),
                        Value: p.BonusValue),

                _ => throw new NotSupportedException($"Unsupported bonus condition {condition.GetType().Name}")
            };
        }
    }
}
