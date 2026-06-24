namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceDataSnapshot(
        int SchemaVersion,
        string ContentVersion,
        DateTimeOffset GeneratedAt,
        string GeneratorVersion,
        IReadOnlyList<ReferenceCharacterDefinition> CharacterDefinitions,
        IReadOnlyList<ReferenceDisasterDefinition> DisasterDefinitions,
        IReadOnlyList<ReferenceLocationDefinition> LocationDefinitions,
        IReadOnlyList<ReferencePodVehicleDefinition> PodVehicleDefinitions,
        IReadOnlyList<ReferenceThunderbirdDefinition> ThunderbirdDefinitions,
        IReadOnlyList<ReferenceMapEdgeDefinition> MapEdgeDefinitions,
        IReadOnlyList<ReferenceFabCardDefinition> FabCardDefinitions,
        IReadOnlyList<ReferenceEventCardDefinition> EventCardDefinitions
    );
}
