using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Calculation
{
    public record RescueCalculationRequest(
        CardCode DisasterCardCode,
        CharacterCode PerformingCharacter,
        IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys,
        IReadOnlyCollection<CardCode> PlayedFabCardCodes,
        IReadOnlyCollection<CardCode> ActiveEventCardCodes);
}
