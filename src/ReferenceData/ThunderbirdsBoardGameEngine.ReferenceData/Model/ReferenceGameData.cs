namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceGameData(
        IReadOnlyList<ReferenceCharacterDefinition> CharacterDefinitions,
        IReadOnlyList<ReferenceDisasterDefinition> DisasterDefinitions,
        IReadOnlyList<ReferenceLocationDefinition> LocationDefinitions,
        IReadOnlyList<ReferencePodVehicleDefinition> PodVehicleDefinitions,
        IReadOnlyList<ReferenceThunderbirdDefinition> ThunderbirdDefinitions
    );
}
