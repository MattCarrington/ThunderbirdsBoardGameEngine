using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces
{
    public interface ISnapshotValidator
    {
        void Validate(ReferenceDataSnapshot snapshot);
    }
}