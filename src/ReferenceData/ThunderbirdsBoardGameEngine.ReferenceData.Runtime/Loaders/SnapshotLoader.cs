using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Loaders
{
    internal sealed class SnapshotLoader
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
            await using var stream = await _provider.GetSnapshotStreamAsync();

            var snapshot = _deserializer.Deserialize(stream);

            Validate(snapshot);

            return snapshot;
        }

        private static void Validate(ReferenceDataSnapshot snapshot)
        {
            if (snapshot.SchemaVersion != SnapshotVersions.SchemaVersion)
            {
                throw new InvalidOperationException(
                    $"Unsupported reference data schema version. " +
                    $"Expected {SnapshotVersions.SchemaVersion}, got {snapshot.SchemaVersion}.");
            }

            if (string.IsNullOrWhiteSpace(snapshot.ContentVersion))
            {
                throw new InvalidOperationException("Reference data content version is missing.");
            }

            // Add these basic checks
            if (snapshot.DisasterDefinitions.Count == 0)
            {
                throw new InvalidOperationException("No disaster definitions loaded.");
            }

            if (snapshot.CharacterDefinitions.Count == 0)
            {
                throw new InvalidOperationException("No character definitions loaded.");
            }

            if (snapshot.LocationDefinitions.Count == 0)
            {
                throw new InvalidOperationException("No location definitions loaded.");
            }
        }
    }
}