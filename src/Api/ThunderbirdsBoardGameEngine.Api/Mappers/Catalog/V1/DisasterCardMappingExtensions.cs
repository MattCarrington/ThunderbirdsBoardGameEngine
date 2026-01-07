using ThunderbirdsBoardGameEngine.Api.Presentation;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Factories;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.Catalog.V1
{
    public static class DisasterCardMappingExtensions
    {
        public static DisasterCardDto ToDto(this DisasterCard disasterCard)
        {
            return new DisasterCardDto
            {
                Id = disasterCard.Id,
                Name = disasterCard.Name,
                DifficultyNumber = disasterCard.DifficultyNumber,
                Location = EnumDisplayHelper.GetDisplayName(disasterCard.Location),
                RescueType = disasterCard.RescueType.ToString(),
                BonusConditions = disasterCard.BonusConditions.Select(ToDto).ToList(),
                Rewards = disasterCard.RewardOptions.Select(ToDto).ToList(),
                Code = disasterCard.Code.ToString()
            };
        }

        public static IReadOnlyList<DisasterCardDto> ToDto(this IEnumerable<DisasterCard> disasterCards)
        {
            return disasterCards.Select(c => c.ToDto()).ToList();
        }

        private static BonusConditionDto ToDto(this BonusCondition bonusCondition)
        {
            var (displayName, key) = bonusCondition switch
            {
                CharacterBonusCondition cb => (EnumDisplayHelper.GetDisplayName(cb.Character), DisasterBonusKeyFactory.ForCharacter(cb.Character)),
                ThunderbirdBonusCondition tb => (EnumDisplayHelper.GetDisplayName(tb.Thunderbird), DisasterBonusKeyFactory.ForThunderbird(tb.Thunderbird)),
                PodVehicleBonusCondition pvb => (EnumDisplayHelper.GetDisplayName(pvb.PodVehicle), DisasterBonusKeyFactory.ForPodVehicle(pvb.PodVehicle)),

                // Unreachable by design:
                // DisasterCard enforces that all BonusCondition instances are known and validated
                // at construction time. This guard exists to fail fast if that invariant is
                // accidentally weakened or a new BonusCondition subtype is introduced
                // without updating the mapping.
                _ => throw new ApplicationValidationException(
                        "Unknown bonus condition type",
                        new Dictionary<string, string[]>
                        {
                            ["BonusCondition.Type"] = new[] { bonusCondition.GetType().Name }
                        })
            };

            var locationText = bonusCondition.Location switch
            {
                BoardLocation.GeoStationaryOrbit => "on Thunderbird 5",
                BoardLocation loc => $"in {EnumDisplayHelper.GetDisplayName(loc)}",
                _ => null
            };

            var description = bonusCondition.Location.HasValue
                ? $"{displayName} (+{bonusCondition.BonusValue}) (if {locationText})"
                : $"{displayName} (+{bonusCondition.BonusValue})";

            return new BonusConditionDto()
            {
                Description = description,
                Key = key.ToString()
            };
        }

        private static RewardDto ToDto(this RewardOption reward)
        {
            return new RewardDto
            {
                DisplayName = reward.IsUserChoice
                    ? "Player Choice"
                    : reward.Token!.Value.ToString() // Safe because of guard
            };
        }
    }
}
