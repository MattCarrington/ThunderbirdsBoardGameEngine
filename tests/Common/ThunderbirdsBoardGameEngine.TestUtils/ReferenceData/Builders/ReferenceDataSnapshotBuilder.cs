using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders
{
    /// <summary>
    /// Fluent builder for creating test snapshots with minimal boilerplate.
    /// </summary>
    public sealed class ReferenceDataSnapshotBuilder
    {
        private int? _schemaVersion;
        private string? _contentVersion;

        private readonly List<ReferenceDisasterDefinition> _disasters = new();
        private readonly List<ReferenceLocationDefinition> _locations = new();
        private readonly List<ReferenceCharacterDefinition> _characters = new();
        private readonly List<ReferenceThunderbirdDefinition> _thunderbirds = new();
        private readonly List<ReferencePodVehicleDefinition> _podVehicles = new();

        public ReferenceDataSnapshotBuilder WithSchemaVersion(int version)
        {
            _schemaVersion = version;
            return this;
        }

        public ReferenceDataSnapshotBuilder WithContentVersion(string version)
        {
            _contentVersion = version;
            return this;
        }

        public ReferenceDataSnapshotBuilder WithLocation(string code, string displayName)
        {
            _locations.Add(new ReferenceLocationDefinition(
                new LocationCode(code),
                displayName));
            return this;
        }

        public ReferenceDataSnapshotBuilder WithCharacter(
            string code,
            string displayName,
            ReferenceCharacterRescueBonus? rescueBonus = null)
        {
            _characters.Add(new ReferenceCharacterDefinition(
                new CharacterCode(code),
                displayName,
                rescueBonus));
            return this;
        }

        public ReferenceDataSnapshotBuilder WithThunderbird(string code, string displayName)
        {
            _thunderbirds.Add(new ReferenceThunderbirdDefinition(
                new ThunderbirdCode(code),
                displayName));
            return this;
        }

        public ReferenceDataSnapshotBuilder WithPodVehicle(string code, string displayName)
        {
            _podVehicles.Add(new ReferencePodVehicleDefinition(
                new PodVehicleCode(code),
                displayName));
            return this;
        }

        public ReferenceDataSnapshotBuilder WithDisaster(
            string code,
            string displayName,
            string locationCode,
            params (string bonusKey, int bonusValue, string? bonusLocation)[] bonuses)
        {
            var bonusList = bonuses
                .Select(b => new ReferenceDisasterBonus(
                    new DisasterBonusKey(b.bonusKey),
                    b.bonusValue,
                    b.bonusLocation != null ? new LocationCode(b.bonusLocation) : null))
                .ToList();

            _disasters.Add(new ReferenceDisasterDefinition(
                new CardCode(code),
                displayName,
                difficultyNumber: 1,
                new LocationCode(locationCode),
                RescueType.Air,
                bonusList,
                new List<ReferenceDisasterReward>
                {
                    new ReferenceDisasterReward.PlayerChoice()
                }));
            return this;
        }

        public ReferenceDataSnapshot Build()
        {
            return new ReferenceDataSnapshot(
                SchemaVersion: _schemaVersion ?? SnapshotVersions.SchemaVersion,
                ContentVersion: _contentVersion ?? SnapshotVersions.ContentVersion,
                GeneratedAt: new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                GeneratorVersion: SnapshotVersions.GeneratorVersion,
                DisasterDefinitions: _disasters,
                LocationDefinitions: _locations,
                CharacterDefinitions: _characters,
                ThunderbirdDefinitions: _thunderbirds,
                PodVehicleDefinitions: _podVehicles);
        }
    }
}