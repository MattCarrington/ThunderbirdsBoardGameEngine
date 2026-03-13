using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Deserializes snapshot data from a specific format.
    /// </summary>
    public interface ISnapshotDeserializer
    {
        ReferenceDataSnapshot Deserialize(Stream stream);
    }
}