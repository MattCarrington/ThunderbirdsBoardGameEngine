using System.Runtime.CompilerServices;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Mappers
{
    public static class DisasterCardMappingExtensions
    {
        public static DisasterCardViewModel ToViewModel(this ReferenceDisasterDefinition disaster)
        {
            return new DisasterCardViewModel(
                Code: disaster.Code.ToString(),
                DisplayName: disaster.DisplayName,
                DifficultyNumber: disaster.DifficultyNumber,
                RescueType: disaster.RescueType.ToString(),
                Location: disaster.Location.ToString(),
                BonusConditions: disaster.Bonuses.Select(bc => bc.ToViewModel()).ToList(),
                Rewards: disaster.Rewards.Select(ro => ro.ToViewModel()).ToList()
            );
        }

        private static BonusConditionViewModel ToViewModel(this ReferenceDisasterBonus bonusCondition)
        {
            string? locationText = null;

            if (bonusCondition.Location.HasValue)
            {
                if (bonusCondition.Location.Value == new LocationCode("geo-stationary-orbit"))
                {
                    locationText = "on Thunderbird 5";
                }
                else
                {
                    locationText = $"in {bonusCondition.Location.Value}";
                }
            }

            var description = bonusCondition.Location.HasValue
                ? $"{bonusCondition.Key.Value} (+{bonusCondition.Value}) (if {locationText})"
                : $"{bonusCondition.Key.Value} (+{bonusCondition.Value})";
            return new BonusConditionViewModel(
                Key: bonusCondition.Key.ToString(),
                Description: description
            );
        }

        private static RewardViewModel ToViewModel(this ReferenceDisasterReward reward)
        {
            if (reward is ReferenceDisasterReward.SpecificToken specificToken)
            {
                return new RewardViewModel(
                    Description: $"{specificToken.Token}"
                );
            }

            return new RewardViewModel(
                Description: "Player Choice"
            );
        }
    }
}
