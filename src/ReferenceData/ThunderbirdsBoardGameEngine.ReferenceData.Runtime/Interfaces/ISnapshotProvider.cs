namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides access to snapshot data from a storage location.
    /// </summary>
    public interface ISnapshotProvider
    {
        Task<Stream> GetSnapshotStreamAsync();
    }
}