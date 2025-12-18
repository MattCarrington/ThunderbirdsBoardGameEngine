using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Providers
{
    public sealed class CatalogRescueContextProvider : IRescueProjectionProvider
    {
        private readonly IDisasterCardCatalog _disasterCardCatalog;

        public CatalogRescueContextProvider(IDisasterCardCatalog disasterCardCatalog)
        {
            _disasterCardCatalog = disasterCardCatalog ?? throw new ArgumentNullException(nameof(disasterCardCatalog));
        }

        public RescueProjection GetRescueContext(int disasterCardId)
        {
            var disasterCard = _disasterCardCatalog.GetById(disasterCardId);

            return new RescueProjection
            (
                DifficultyNumber: disasterCard.DifficultyNumber,
                Bonuses: disasterCard.BonusConditions.Select(ProjectBonus).ToList()
            );
        }

        private static RescueBonus ProjectBonus(BonusCondition condition)
        {
            return condition switch
            {
                CharacterBonusCondition c =>
                    new RescueBonus(
                        Key: c.Character.ToString().ToLowerInvariant(),
                        Value: c.BonusValue),

                ThunderbirdBonusCondition t =>
                    new RescueBonus(
                        Key: t.Thunderbird.ToString().ToLowerInvariant(),
                        Value: t.BonusValue),

                PodVehicleBonusCondition p =>
                    new RescueBonus(
                        Key: p.PodVehicle.ToString().ToLowerInvariant(),
                        Value: p.BonusValue),

                _ => throw new NotSupportedException($"Unsupported bonus condition {condition.GetType().Name}")
            };
        }
    }
}
