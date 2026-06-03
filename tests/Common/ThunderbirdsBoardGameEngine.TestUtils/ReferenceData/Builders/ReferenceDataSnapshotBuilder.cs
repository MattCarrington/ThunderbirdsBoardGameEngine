using ThunderbirdsBoardGameEngine.ReferenceData;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;

/// <summary>
/// Fluent builder for creating test snapshots with minimal boilerplate.
/// </summary>
public sealed class ReferenceDataSnapshotBuilder
{
    private int? _schemaVersion;
    private string? _contentVersion;
    private DateTimeOffset? _generatedAt;
    private string? _generatorVersion;

    private readonly List<ReferenceDisasterDefinition> _disasters = [];
    private readonly List<ReferenceLocationDefinition> _locations = [];
    private readonly List<ReferenceCharacterDefinition> _characters = [];
    private readonly List<ReferenceThunderbirdDefinition> _thunderbirds = [];
    private readonly List<ReferencePodVehicleDefinition> _podVehicles = [];

    public static ReferenceDataSnapshotBuilder Valid()
    {
        return new ReferenceDataSnapshotBuilder()
            .WithLocation("location-1", "Location 1")
            .WithCharacter("character-1", "Character 1")
            .WithDisaster(
                "disaster-1",
                "Disaster 1",
                "location-1",
                ("character-1", 1, null));
    }

    public ReferenceDataSnapshotBuilder WithSchemaVersion(int version)
    {
        _schemaVersion = version;
        return this;
    }

    public ReferenceDataSnapshotBuilder WithContentVersion(string? version)
    {
        _contentVersion = version;
        return this;
    }

    public ReferenceDataSnapshotBuilder WithGeneratedAt(DateTimeOffset generatedAt)
    {
        _generatedAt = generatedAt;
        return this;
    }

    public ReferenceDataSnapshotBuilder WithGeneratorVersion(string version)
    {
        _generatorVersion = version;
        return this;
    }

    public ReferenceDataSnapshotBuilder WithLocation(
        string code,
        string displayName,
        MovementDomain domain = MovementDomain.Earth)
    {
        _locations.Add(new ReferenceLocationDefinition(
            new LocationCode(code),
            displayName,
            domain));

        return this;
    }

    public ReferenceDataSnapshotBuilder WithoutLocations()
    {
        _locations.Clear();
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

    public ReferenceDataSnapshotBuilder WithoutCharacters()
    {
        _characters.Clear();
        return this;
    }

    public ReferenceDataSnapshotBuilder WithThunderbird(
        string code,
        string displayName,
        MovementDomain domain = MovementDomain.Earth,
        int topSpeed = 0)
    {
        _thunderbirds.Add(new ReferenceThunderbirdDefinition(
            new ThunderbirdCode(code),
            displayName,
            domain,
            topSpeed));

        return this;
    }

    public ReferenceDataSnapshotBuilder WithoutThunderbirds()
    {
        _thunderbirds.Clear();
        return this;
    }

    public ReferenceDataSnapshotBuilder WithPodVehicle(
        string code,
        string displayName)
    {
        _podVehicles.Add(new ReferencePodVehicleDefinition(
            new PodVehicleCode(code),
            displayName));

        return this;
    }

    public ReferenceDataSnapshotBuilder WithoutPodVehicles()
    {
        _podVehicles.Clear();
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
                b.bonusLocation is not null
                    ? new LocationCode(b.bonusLocation)
                    : null))
            .ToList();

        _disasters.Add(new ReferenceDisasterDefinition(
            code: new CardCode(code),
            displayName: displayName,
            difficultyNumber: 1,
            location: new LocationCode(locationCode),
            rescueType: RescueType.Air,
            bonuses: bonusList,
            rewards:
            [
                new ReferenceDisasterReward.PlayerChoice()
            ]));

        return this;
    }

    public ReferenceDataSnapshotBuilder WithDisaster(
        ReferenceDisasterDefinition disaster)
    {
        _disasters.Add(disaster);
        return this;
    }

    public ReferenceDataSnapshotBuilder WithoutDisasters()
    {
        _disasters.Clear();
        return this;
    }

    public ReferenceDataSnapshot Build()
    {
        return new ReferenceDataSnapshot(
            SchemaVersion: _schemaVersion ?? SnapshotVersions.SchemaVersion,
            ContentVersion: _contentVersion ?? SnapshotVersions.ContentVersion,
            GeneratedAt: _generatedAt ?? new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            GeneratorVersion: _generatorVersion ?? SnapshotVersions.GeneratorVersion,
            DisasterDefinitions: _disasters,
            LocationDefinitions: _locations,
            CharacterDefinitions: _characters,
            ThunderbirdDefinitions: _thunderbirds,
            PodVehicleDefinitions: _podVehicles);
    }
}