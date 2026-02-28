namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceDataSnapshot(
        int SchemaVersion,
        string ContentVersion,
        IReadOnlyList<ReferenceCharacterDefinition> CharacterDefinitions,
        IReadOnlyList<ReferenceDisasterDefinition> DisasterDefinitions,
        IReadOnlyList<ReferenceLocationDefinition> LocationDefinitions,
        IReadOnlyList<ReferencePodVehicleDefinition> PodVehicleDefinitions,
        IReadOnlyList<ReferenceThunderbirdDefinition> ThunderbirdDefinitions
    );
}
