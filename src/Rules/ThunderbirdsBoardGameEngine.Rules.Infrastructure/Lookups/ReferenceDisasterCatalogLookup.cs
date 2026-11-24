using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceDisasterCatalogLookup : IDisasterCatalogLookup
    {
        private readonly IDisasterDefinitionCatalog _disasterCardReferenceSource;

        public ReferenceDisasterCatalogLookup(IDisasterDefinitionCatalog disasterCardReferenceSource)
        {
            _disasterCardReferenceSource = disasterCardReferenceSource ?? throw new ArgumentNullException(nameof(disasterCardReferenceSource));
        }

        public DisasterContribution GetDisasterRescueContribution(CardCode disasterCode)
        {
            if (!_disasterCardReferenceSource.TryGetByCode(disasterCode, out var disasterDefinition))
            {
                throw new ReferenceDataNotFoundException("Disaster", disasterCode.ToString());
            }

            return new DisasterContribution
            (
                difficultyNumber: disasterDefinition.DifficultyNumber,
                availableBonuses: disasterDefinition.Bonuses.Select(ProjectBonus).ToList(),
                rescueType: disasterDefinition.RescueType
            );
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
