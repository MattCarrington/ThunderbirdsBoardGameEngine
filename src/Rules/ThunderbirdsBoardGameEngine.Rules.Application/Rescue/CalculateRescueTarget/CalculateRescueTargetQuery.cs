using MediatR;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(int DisasterCardId, IReadOnlyCollection<DisasterBonusKey> AppliedBonusKeys)
        : IRequest<CalculateRescueTargetResponse>;
}