using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source
{
    /// <summary>
    /// Provides reference data from a source for compilation.
    /// </summary>
    public interface IReferenceDataSource
    {
        /// <summary>
        /// Loads reference data and returns a compilation context.
        /// </summary>
        CompilationContext Load();
    }
}