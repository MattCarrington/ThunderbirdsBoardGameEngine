using MediatR;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(CardCode DisasterCardCode, RescueCalculationInput RescueCalculationInput)
        : IRequest<CalculateRescueTargetResponse>;
}