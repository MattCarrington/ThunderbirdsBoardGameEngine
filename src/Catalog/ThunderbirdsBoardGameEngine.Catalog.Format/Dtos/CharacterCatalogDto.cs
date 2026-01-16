namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public record CharacterCatalogDto
    {
        public required string Key { get; init; } = default!;

        public required List<CharacterRescueBonusCatalogDto> RescueBonuses { get; init; }
    }
}
