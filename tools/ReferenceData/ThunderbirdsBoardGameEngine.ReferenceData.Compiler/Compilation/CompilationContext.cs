using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class CompilationContext
    {
        public IReadOnlyList<DisasterInput> Disasters { get; init; } = [];
    }
}
