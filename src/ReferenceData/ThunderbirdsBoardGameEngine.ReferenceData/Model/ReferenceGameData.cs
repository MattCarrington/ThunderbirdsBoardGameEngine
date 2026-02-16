namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceGameData(
        IReadOnlyList<ReferenceCharacterDefinition> CharacterDefinitions,
        IReadOnlyList<ReferenceDisasterBonus> DisasterDefinitions,
        IReadOnlyList<ReferenceLocationDefinition> LocationDefinitions,
        IReadOnlyList<ReferencePodVehicleDefinition> PodVehicleDefinitions,
        IReadOnlyList<ReferenceThunderbirdDefinition> ThunderbirdDefinitions
    );
}
