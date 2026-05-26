using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IMapEdgeDefinitionLookup
    {
        IReadOnlyCollection<ReferenceMapEdgeDefinition> GetAll();
    }
}