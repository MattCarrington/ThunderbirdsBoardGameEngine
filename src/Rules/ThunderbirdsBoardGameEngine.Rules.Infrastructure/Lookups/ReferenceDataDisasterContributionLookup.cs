using ThunderbirdsBoardGameEngine.Catalog.Application.Factories;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceDataDisasterContributionLookup : IDisasterContributionLookup
    {
        private readonly IDisasterDefinitionCatalog _disasterCardReferenceSource;

        public ReferenceDataDisasterContributionLookup(IDisasterDefinitionCatalog disasterCardReferenceSource)
        {
            _disasterCardReferenceSource = disasterCardReferenceSource ?? throw new ArgumentNullException(nameof(disasterCardReferenceSource));
        }

        public DisasterContribution GetDisasterContribution(CardCode disasterCode)
        {
            try
            {
                var disasterCard = _disasterCardReferenceSource.GetByCode(disasterCode);

                return new DisasterContribution
                (
                    difficultyNumber: disasterCard.DifficultyNumber,
                    availableBonuses: disasterCard.Bonuses.Select(ProjectBonus).ToList(),
                    rescueType: disasterCard.RescueType
                );
            }
            catch (KeyNotFoundException)
            {
                throw new ReferenceDataNotFoundException("Disaster", disasterCode.ToString());
            }
        }

        private static DisasterBonus ProjectBonus(ReferenceDisasterBonus condition)
        {
            return new DisasterBonus
            (
                Key: condition.Key,
                Value: condition.Value
            );
        }
    }
}
