using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class CompilationContext
    {
        public List<DisasterInput> Disasters { get; init; } = new();

        public List<LocationInput> Locations { get; init; } = new();

        public List<CharacterInput> Characters { get; init; } = new();

        public List<ThunderbirdInput> Thunderbirds { get; init; } = new();

        public List<PodVehicleInput> PodVehicles { get; init; } = new();

        public List<MapEdgeInput> MapEdges { get; init; } = new();

        public List<FabCardInput> FabCards { get; init; } = new();

        public List<EventCardInput> EventCards { get; init; } = new();
    }
}
