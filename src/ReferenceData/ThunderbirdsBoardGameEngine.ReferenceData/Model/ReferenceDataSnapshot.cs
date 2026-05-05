namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceDataSnapshot(
        int SchemaVersion,
        string ContentVersion,
        DateTime GeneratedAt,
        string GeneratorVersion,
        IReadOnlyList<ReferenceCharacterDefinition> CharacterDefinitions,
        IReadOnlyList<ReferenceDisasterDefinition> DisasterDefinitions,
        IReadOnlyList<ReferenceLocationDefinition> LocationDefinitions,
        IReadOnlyList<ReferencePodVehicleDefinition> PodVehicleDefinitions,
        IReadOnlyList<ReferenceThunderbirdDefinition> ThunderbirdDefinitions
    );
}
