using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Providers
{
    public sealed class CatalogRescueContextProvider : IRescueContextProvider
    {
        private readonly IDisasterCardCatalog _disasterCardCatalog;

        public CatalogRescueContextProvider(IDisasterCardCatalog disasterCardCatalog)
        {
            _disasterCardCatalog = disasterCardCatalog ?? throw new ArgumentNullException(nameof(disasterCardCatalog));
        }

        public RescueContext GetRescueContext(int disasterCardId)
        {
            var disasterCard = _disasterCardCatalog.GetById(disasterCardId);

            return new RescueContext
            (
                DifficultyNumber: disasterCard.DifficultyNumber,
                Bonuses: disasterCard.BonusConditions.Select(ProjectBonus).ToList()
            );
        }

        private static RescueContextBonus ProjectBonus(BonusCondition condition)
        {
            return condition switch
            {
                CharacterBonusCondition c =>
                    new RescueContextBonus(
                        Key: c.Character.ToString().ToLowerInvariant(),
                        Value: c.BonusValue),

                ThunderbirdBonusCondition t =>
                    new RescueContextBonus(
                        Key: t.Thunderbird.ToString().ToLowerInvariant(),
                        Value: t.BonusValue),

                PodVehicleBonusCondition p =>
                    new RescueContextBonus(
                        Key: p.PodVehicle.ToString().ToLowerInvariant(),
                        Value: p.BonusValue),

                _ => throw new NotSupportedException($"Unsupported bonus condition {condition.GetType().Name}")
            };
        }
    }
}
