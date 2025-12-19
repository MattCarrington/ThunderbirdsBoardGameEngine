using MediatR;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(int DisasterCardId, IReadOnlyCollection<string> AppliedBonusKeys)
        : IRequest<CalculateRescueTargetResponse>;
}