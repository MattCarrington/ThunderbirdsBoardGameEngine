using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Contributions;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal class ReferenceThunderbirdDefinitionLookup : IThunderbirdMovementDefinitionLookup
    {
        private readonly IThunderbirdDefinitionCatalog _thunderbirdDefinitionCatalog;

        public ReferenceThunderbirdDefinitionLookup(IThunderbirdDefinitionCatalog thunderbirdDefinitionCatalog)
        {
            _thunderbirdDefinitionCatalog = thunderbirdDefinitionCatalog;
        }

        public ThunderbirdMovementContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode)
        {
            if (!_thunderbirdDefinitionCatalog.TryGetByCode(thunderbirdCode, out var thunderbird))
            {
                throw new ReferenceDataNotFoundException("Thunderbird", thunderbirdCode.Value);
            }

            return new ThunderbirdMovementContribution(
                Key: thunderbird.Code,
                TraversalDomain: thunderbird.Domain,
                TopSpeed: thunderbird.TopSpeed);
        }
    }
}
