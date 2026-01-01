using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1
{
    public static class RescueMappingExtensions
    {
        public static CalculateRescueTargetQuery ToQuery(this CalculateRescueTargetRequestDto request, int disasterCardId)
        {
            return new CalculateRescueTargetQuery
            (
                DisasterCardId: disasterCardId,
                RescueCalculationInput: new(request.AppliedBonusKeys.Select(k => new DisasterBonusKey(k)).ToList())
            );
        }

        public static CalculateRescueTargetResponseDto ToDto(this CalculateRescueTargetResponse response)
        {
            return new CalculateRescueTargetResponseDto
            {
                TargetNumber = response.TargetNumber,
                TotalBonus = response.TotalBonus,
                AppliedBonuses = response.AppliedBonuses.Select(b => b.ToDto()).ToList()
            };
        }

        private static AppliedBonusDto ToDto(this DisasterBonus bonus)
        {
            return new AppliedBonusDto
            {
                BonusKey = bonus.Key.Value,
                BonusValue = bonus.Value
            };
        }
    }
}
