using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces
{
    public interface ISnapshotWriter
    {
        void Write(ReferenceDataSnapshot snapshot);
    }
}