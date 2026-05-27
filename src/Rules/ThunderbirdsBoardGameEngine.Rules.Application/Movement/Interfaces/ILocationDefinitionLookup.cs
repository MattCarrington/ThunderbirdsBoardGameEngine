using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface ILocationDefinitionLookup
    {
        IReadOnlyCollection<LocationContribution> GetAllLocationContributions();
    }
}