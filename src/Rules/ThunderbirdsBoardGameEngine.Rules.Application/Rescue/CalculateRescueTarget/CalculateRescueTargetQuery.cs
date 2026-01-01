using MediatR;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(int DisasterCardId, RescueCalculationInput RescueCalculationInput)
        : IRequest<CalculateRescueTargetResponse>;
}