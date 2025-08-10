using ThunderbirdsBoardGameEngine.GameData.Api.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Serialization.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Mappers.V1
{
    public static class DisasterCardMappingExtensions
    {
        public static BonusConditionDto ToDto(this BonusCondition bonusCondition)
        {
            var displayName = bonusCondition switch
            {
                CharacterBonusCondition cb => EnumDisplayHelper.GetDisplayName(cb.Character),
                ThunderbirdBonusCondition tb => EnumDisplayHelper.GetDisplayName(tb.Thunderbird),
                PodVehicleBonusCondition pvb => EnumDisplayHelper.GetDisplayName(pvb.PodVehicle),
                _ => throw new InvalidBonusConditionTypeException("Unknown bonus condition")
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
                Description = description
            };
        }

        public static RewardDto ToDto(this RewardOption reward)
        {
            if (!reward.IsUserChoice && reward.SpecifiedToken == null)
            {
                throw new InvalidRewardConditionException("SpecifiedToken must be set for non-user-choice rewards");
            }

            return new RewardDto
            {
                DisplayName = reward.IsUserChoice
                    ? "Player Choice"
                    : reward.SpecifiedToken!.Value.ToString() // Safe because of guard
            };
        }

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
                Rewards = disasterCard.RewardOptions.Select(ToDto).ToList()
            };
        }

        public static IReadOnlyList<DisasterCardDto> ToDto(this IEnumerable<DisasterCard> disasterCards)
        {
            return disasterCards.Select(c => c.ToDto()).ToList();
        }
    }
}
