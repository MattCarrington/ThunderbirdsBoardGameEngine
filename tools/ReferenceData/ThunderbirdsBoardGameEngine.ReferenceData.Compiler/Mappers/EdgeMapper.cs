using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class EdgeMapper  // Simplifying name to EdgeMapper for clarity as MapEdgeMapper is a bit confusing in this context
    {
        private readonly LocationCodeResolver _locationCodeResolver;

        public EdgeMapper(LocationCodeResolver locationCodeResolver)
        {
            _locationCodeResolver = locationCodeResolver;
        }

        public IEnumerable<ReferenceMapEdgeDefinition> Map(IEnumerable<MapEdgeInput> inputs)
        {
            return inputs.Select(MapGameBoardEdges);
        }

        private ReferenceMapEdgeDefinition MapGameBoardEdges(MapEdgeInput input)    // Renamed method to MapGameBoardEdges for clarity as MapMapEdges was very confusing in this context
        {
            return new ReferenceMapEdgeDefinition(
                edge1: _locationCodeResolver.Resolve(input.Edge1),
                edge2: _locationCodeResolver.Resolve(input.Edge2),
                edgeType: Enum.Parse<MovementDomain>(input.Domain, ignoreCase: true));
        }
    }
}
