using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IReferenceThunderbirdsDefinitionLookup
    {
        ThunderbirdContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode);
    }
}