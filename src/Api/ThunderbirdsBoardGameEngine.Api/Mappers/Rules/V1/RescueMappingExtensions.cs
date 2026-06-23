using ThunderbirdsBoardGameEngine.Api.Exceptions;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.CalculateRescueTarget.V1;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.Rules.V1
{
    public static class RescueMappingExtensions
    {
        public static CalculateRescueTargetQuery ToQuery(this CalculateRescueTargetRequestDto request, string disasterCardCode)
        {
            if (string.IsNullOrWhiteSpace(request.PerformingCharacterKey))
            {
                throw new BadRequestException("Performing character key must be provided.");
            }

            ValidateOptionalStringList(request.PresentDisasterBonusKeys, nameof(request.PresentDisasterBonusKeys));
            ValidateOptionalStringList(request.PlayedFabCardKeys, nameof(request.PlayedFabCardKeys));
            ValidateOptionalStringList(request.ActiveEventCardKeys, nameof(request.ActiveEventCardKeys));

            return new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode(disasterCardCode),
                PerformingCharacter: new CharacterCode(request.PerformingCharacterKey),
                PresentDisasterBonusKeys: request.PresentDisasterBonusKeys.Select(k => new DisasterBonusKey(k)).ToList(),
                PlayedFabCardCodes: request.PlayedFabCardKeys.Select(c => new CardCode(c)).ToList(),
                ActiveEventCardCodes: request.ActiveEventCardKeys.Select(c => new CardCode(c)).ToList()
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
                SourceType = GetSourceType(bonus.SourceType)
            };
        }

        private static string GetSourceType(SourceType sourceType)
        {
            return sourceType switch
            {
                SourceType.DisasterCard => "disaster-card",
                SourceType.CharacterAbility => "character-ability",
                SourceType.FabCard => "fab-card",
                SourceType.EventCard => "event-card",
                _ => throw new InvalidOperationException($"Unhandled SourceType '{sourceType}'")
            };
        }

        private static void ValidateOptionalStringList(IEnumerable<string> list, string propertyName)
        {
            if (list is null)
            {
                throw new BadRequestException($"{propertyName} cannot be null.");
            }

            if (list.Any(string.IsNullOrWhiteSpace))
            {
                throw new BadRequestException($"{propertyName} cannot contain null or whitespace values.");
            }
        }
    }
}
