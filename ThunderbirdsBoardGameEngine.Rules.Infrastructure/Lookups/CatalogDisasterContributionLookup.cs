using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
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

        public DisasterContribution GetDisasterContribution(int disasterCardId)
        {
            var disasterCard = _disasterCardReferenceSource.GetById(disasterCardId);

            return new DisasterContribution
            (
                DifficultyNumber: disasterCard.DifficultyNumber,
                Bonuses: disasterCard.BonusConditions.Select(ProjectBonus).ToList()
            );
        }

        private static DisasterBonus ProjectBonus(BonusCondition condition)
        {
            return condition switch
            {
                CharacterBonusCondition c =>
                    new DisasterBonus(
                        Key: c.Character.ToString().ToLowerInvariant(),
                        Value: c.BonusValue),

                ThunderbirdBonusCondition t =>
                    new DisasterBonus(
                        Key: t.Thunderbird.ToString().ToLowerInvariant(),
                        Value: t.BonusValue),

                PodVehicleBonusCondition p =>
                    new DisasterBonus(
                        Key: p.PodVehicle.ToString().ToLowerInvariant(),
                        Value: p.BonusValue),

                _ => throw new NotSupportedException($"Unsupported bonus condition {condition.GetType().Name}")
            };
        }
    }
}
