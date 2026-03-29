using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation.Validators
{
    public interface ISnapshotValidator
    {
        void Validate(ReferenceDataSnapshot snapshot);
    }
}