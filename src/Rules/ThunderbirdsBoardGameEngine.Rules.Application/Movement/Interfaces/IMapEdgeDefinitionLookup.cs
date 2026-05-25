using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IMapEdgeDefinitionLookup
    {
        IEnumerable<ReferenceMapEdgeDefinition> GetAll();
    }
}