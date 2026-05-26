using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal class ReferenceThunderbirdsDefinitionLookup : IReferenceThunderbirdsDefinitionLookup
    {
        private readonly IThunderbirdDefinitionCatalog _thunderbirdDefinitionCatalog;

        public ReferenceThunderbirdsDefinitionLookup(IThunderbirdDefinitionCatalog thunderbirdDefinitionCatalog)
        {
            _thunderbirdDefinitionCatalog = thunderbirdDefinitionCatalog;
        }

        public ThunderbirdContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode)
        {
            try
            {
                var thunderbirdDefinition = _thunderbirdDefinitionCatalog.GetByCode(thunderbirdCode);

                return new ThunderbirdContribution(
                    Key: thunderbirdDefinition.Code,
                    TraversalDomain: thunderbirdDefinition.Domain
                );
            }
            catch (KeyNotFoundException)
            {
                throw new ReferenceDataNotFoundException("Thunderbird", thunderbirdCode.ToString());
            }
        }
    }
}
