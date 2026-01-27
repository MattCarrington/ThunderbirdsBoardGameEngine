using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1
{
    public static class RescueMappingExtensions
    {
        public static CalculateRescueTargetQuery ToQuery(this CalculateRescueTargetRequestDto request, string disasterCardCode)
        {
            return new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode(disasterCardCode),
                RescueCalculationInput: new(request.PresentDisasterBonusKeys.Select(k => new DisasterBonusKey(k)).ToList())
            );
        }

        public static CalculateRescueTargetResponseDto ToDto(this CalculateRescueTargetResponse response)
        {
            return new CalculateRescueTargetResponseDto
            {
                TargetNumber = response.TargetNumber,
                TotalBonus = response.TotalBonus,
                AppliedDisasterBonuses = response.AppliedBonuses.Select(b => b.ToDto()).ToList()
            };
        }

        private static AppliedDisasterBonusDto ToDto(this AppliedRescueModifier bonus)
        {
            return new AppliedDisasterBonusDto
            {
                BonusKey = bonus.Key,
                BonusValue = bonus.Value,
                SourceType = "disaster-card"    // TODO: Currently, all bonuses come from disaster cards
            };
        }
    }
}
