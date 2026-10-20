using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.CharacterTransfer.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Contributions;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal class ReferenceThunderbirdDefinitionLookup : IThunderbirdMovementDefinitionLookup, IThunderbirdTransferDefinitionLookup
    {
        private readonly IThunderbirdDefinitionCatalog _thunderbirdDefinitionCatalog;

        public ReferenceThunderbirdDefinitionLookup(IThunderbirdDefinitionCatalog thunderbirdDefinitionCatalog)
        {
            _thunderbirdDefinitionCatalog = thunderbirdDefinitionCatalog;
        }

        public ThunderbirdMovementContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode)
        {
            var thunderbird = GetThunderbirdDefinition(thunderbirdCode);

            return new ThunderbirdMovementContribution(
                Key: thunderbird.Code,
                TraversalDomain: thunderbird.Domain,
                TopSpeed: thunderbird.TopSpeed);
        }

        public int GetThunderbirdCapacity(ThunderbirdCode thunderbirdCode)
        {
            var thunderbird = GetThunderbirdDefinition(thunderbirdCode);

            return thunderbird.CharacterCapacity;
        }

        private ReferenceThunderbirdDefinition GetThunderbirdDefinition(ThunderbirdCode thunderbirdCode)
        {
            if (!_thunderbirdDefinitionCatalog.TryGetByCode(thunderbirdCode, out var thunderbird))
            {
                throw new ReferenceDataNotFoundException("Thunderbird", thunderbirdCode.Value);
            }

            return thunderbird;
        }
    }
}
