using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IThunderbirdsDefinitionLookup
    {
        ThunderbirdContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode);
    }
}