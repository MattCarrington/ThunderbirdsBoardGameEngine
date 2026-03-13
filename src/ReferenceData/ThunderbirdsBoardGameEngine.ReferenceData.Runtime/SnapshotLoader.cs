using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime
{
    public class SnapshotLoader
    {
        private readonly ISnapshotProvider _provider;
        private readonly ISnapshotDeserializer _deserializer;

        public SnapshotLoader(ISnapshotProvider provider, ISnapshotDeserializer deserializer)
        {
            _provider = provider;
            _deserializer = deserializer;
        }

        public async Task<ReferenceDataSnapshot> LoadAsync()
        {
            using var stream = await _provider.GetSnapshotStreamAsync();
            return _deserializer.Deserialize(stream);
        }
    }
}