using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(CardCode DisasterCardCode, CharacterCode PerformingCharacter, IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys)
        : IRequest<CalculateRescueTargetResponse>;
}