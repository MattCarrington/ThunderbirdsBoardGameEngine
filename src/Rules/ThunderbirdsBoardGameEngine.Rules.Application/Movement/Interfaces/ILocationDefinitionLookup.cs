using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Contributions;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface ILocationDefinitionLookup
    {
        IReadOnlyCollection<LocationContribution> GetAllLocationContributions();

        bool Exists(LocationCode locationCode);
    }
}