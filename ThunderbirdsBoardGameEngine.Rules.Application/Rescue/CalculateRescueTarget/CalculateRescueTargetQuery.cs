using MediatR;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(int DisasterCardId, IReadOnlyCollection<DisasterBonusKey> AppliedBonusKeys)
        : IRequest<CalculateRescueTargetResponse>;
}