using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Contributions;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IThunderbirdsDefinitionLookup
    {
        ThunderbirdContribution GetThunderbirdMovementContribution(ThunderbirdCode thunderbirdCode);
    }
}