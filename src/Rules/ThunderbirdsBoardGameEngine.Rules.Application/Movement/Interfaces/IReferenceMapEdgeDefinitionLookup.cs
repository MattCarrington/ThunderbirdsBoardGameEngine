using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces
{
    public interface IReferenceMapEdgeDefinitionLookup
    {
        IEnumerable<ReferenceMapEdgeDefinition> GetAll();
    }
}