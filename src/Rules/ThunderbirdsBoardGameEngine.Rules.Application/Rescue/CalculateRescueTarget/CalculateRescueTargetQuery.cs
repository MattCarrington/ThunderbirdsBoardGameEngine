using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(
        CardCode DisasterCardCode,
        CharacterCode PerformingCharacter,
        IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys,
        IReadOnlyCollection<CardCode> PlayedFabCardCodes,
        IReadOnlyCollection<CardCode> ActiveEventCardCodes)
        : IRequest<CalculateRescueTargetResponse>;
}