using Microsoft.Extensions.Logging;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Loaders
{
    internal sealed class SnapshotLoader
    {
        private readonly ISnapshotProvider _provider;
        private readonly ISnapshotDeserializer _deserializer;
        private readonly ILogger<SnapshotLoader> _logger;

        public SnapshotLoader(ISnapshotProvider provider, ISnapshotDeserializer deserializer, ILogger<SnapshotLoader> logger)
        {
            _provider = provider;
            _deserializer = deserializer;
            _logger = logger;
        }

        public async Task<ReferenceDataSnapshot> LoadAsync()
        {
            await using var stream = await _provider.GetSnapshotStreamAsync();

            var snapshot = _deserializer.Deserialize(stream);

            Validate(snapshot);

            return snapshot;
        }

        private void Validate(ReferenceDataSnapshot snapshot)
        {
            if (snapshot.SchemaVersion != SnapshotVersions.SchemaVersion)
            {
                _logger.LogCritical(
                    "Unsupported reference data schema version. Expected {ExpectedVersion}, got {ActualVersion}.",
                    SnapshotVersions.SchemaVersion, snapshot.SchemaVersion);
                throw new InvalidOperationException(
                    $"Unsupported reference data schema version. " +
                    $"Expected {SnapshotVersions.SchemaVersion}, got {snapshot.SchemaVersion}.");
            }

            if (string.IsNullOrWhiteSpace(snapshot.ContentVersion))
            {
                _logger.LogCritical("Reference data content version is missing.");
                throw new InvalidOperationException("Reference data content version is missing.");
            }

            if (snapshot.DisasterDefinitions.Count == 0)
            {
                _logger.LogCritical("No disaster definitions loaded.");
                throw new InvalidOperationException("No disaster definitions loaded.");
            }

            if (snapshot.CharacterDefinitions.Count == 0)
            {
                _logger.LogCritical("No character definitions loaded.");
                throw new InvalidOperationException("No character definitions loaded.");
            }

            if (snapshot.LocationDefinitions.Count == 0)
            {
                _logger.LogCritical("No location definitions loaded.");
                throw new InvalidOperationException("No location definitions loaded.");
            }
        }
    }
}