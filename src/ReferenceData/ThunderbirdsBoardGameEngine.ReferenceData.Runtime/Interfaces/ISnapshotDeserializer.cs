using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Deserializes snapshot data from a specific format.
    /// </summary>
    internal interface ISnapshotDeserializer
    {
        ReferenceDataSnapshot Deserialize(Stream stream);
    }
}