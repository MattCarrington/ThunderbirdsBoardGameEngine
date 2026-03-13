using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class CompilationContext
    {
        public List<DisasterInput> Disasters { get; init; } = new();

        public List<LocationInput> Locations { get; init; } = new();
    }
}
