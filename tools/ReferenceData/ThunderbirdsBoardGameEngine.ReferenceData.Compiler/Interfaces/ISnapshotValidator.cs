using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces
{
    public interface ISnapshotValidator
    {
        void Validate(ReferenceDataSnapshot snapshot);
    }
}