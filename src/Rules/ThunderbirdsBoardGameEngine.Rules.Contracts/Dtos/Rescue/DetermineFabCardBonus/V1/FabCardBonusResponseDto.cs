namespace ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.DetermineFabCardBonus.V1
{
    /// <summary>
    /// Represents the appropriate FAB card bonus for a given disaster card. This DTO contains the code of the FAB card and the associated bonus value that can be applied during a rescue operation.
    /// </summary>
    public class FabCardBonusResponseDto
    {
        /// <summary>
        /// Gets the code identifying the FAB card.
        /// </summary>
        public string FabCardCode { get; init; } = string.Empty;

        /// <summary>
        /// Gets the bonus value.
        /// </summary>
        public int BonusValue { get; init; }
    }
}
