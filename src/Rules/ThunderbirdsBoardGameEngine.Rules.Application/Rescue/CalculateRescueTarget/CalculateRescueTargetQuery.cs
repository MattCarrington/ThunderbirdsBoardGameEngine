using MediatR;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public record CalculateRescueTargetQuery(CardCode DisasterCardCode, RescueCalculationInput RescueCalculationInput, CharacterCode PerformingCharacter)
        : IRequest<CalculateRescueTargetResponse>;
}