using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public record RescueCalculationRequest(
        CardCode DisasterCardCode,
        CharacterCode PerformingCharacter,
        IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys,
        IReadOnlyCollection<CardCode> PlayedFabCardCodes,
        IReadOnlyCollection<CardCode> ActiveEventCardCodes);
}
