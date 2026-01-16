namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public record CharacterRescueBonusCatalogDto
    {
        public required string RescueType { get; init; } = default!;

        public required int BonusValue { get; init; }
    }
}
