using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IMapEdgeDefinitionLookup
    {
        IReadOnlyCollection<ReferenceMapEdgeDefinition> GetAll();
    }
}