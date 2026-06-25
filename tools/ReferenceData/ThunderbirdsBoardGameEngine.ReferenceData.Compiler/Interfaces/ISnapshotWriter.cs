using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces
{
    public interface ISnapshotWriter
    {
        void Write(ReferenceDataSnapshot snapshot);
    }
}