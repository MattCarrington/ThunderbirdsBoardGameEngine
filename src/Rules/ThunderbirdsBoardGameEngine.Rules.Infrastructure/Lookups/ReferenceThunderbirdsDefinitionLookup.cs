using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal class ReferenceThunderbirdsDefinitionLookup : IThunderbirdsDefinitionLookup
    {
        private readonly IThunderbirdDefinitionCatalog _thunderbirdDefinitionCatalog;

        public ReferenceThunderbirdsDefinitionLookup(IThunderbirdDefinitionCatalog thunderbirdDefinitionCatalog)
        {
            _thunderbirdDefinitionCatalog = thunderbirdDefinitionCatalog;
        }

        public ThunderbirdContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode)
        {
            if (!_thunderbirdDefinitionCatalog.TryGetByCode(thunderbirdCode, out var thunderbird))
            {
                throw new ReferenceDataNotFoundException("Thunderbird", thunderbirdCode.Value);
            }

            return new ThunderbirdContribution(
                Key: thunderbird.Code,
                TraversalDomain: thunderbird.Domain,
                TopSpeed: thunderbird.TopSpeed);
        }
    }
}
