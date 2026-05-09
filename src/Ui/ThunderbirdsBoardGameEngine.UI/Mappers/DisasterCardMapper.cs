using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Mappers
{
    public sealed class DisasterCardMapper
    {
        private readonly ILocationDefinitionCatalog _locationDefinitionCatalog;

        public DisasterCardMapper(ILocationDefinitionCatalog locationDefinitionCatalog)
        {
            _locationDefinitionCatalog = locationDefinitionCatalog;
        }

        public DisasterCardViewModel Map(ReferenceDisasterDefinition disaster)
        {
            return new DisasterCardViewModel(
                Code: disaster.Code.ToString(),
                DisplayName: disaster.DisplayName,
                DifficultyNumber: disaster.DifficultyNumber,
                RescueType: disaster.RescueType.ToString(),
                Location: _locationDefinitionCatalog.GetByCode(disaster.Location).DisplayName,
                BonusConditions: disaster.Bonuses.Select(MapBonus).ToList(),
                Rewards: disaster.Rewards.Select(MapReward).ToList()
            );
        }

        private BonusConditionViewModel MapBonus(ReferenceDisasterBonus bonusCondition)
        {
            string? locationText = null;

            if (bonusCondition.Location.HasValue)
            {
                locationText = bonusCondition.Location.Value == new LocationCode("geo-stationary-orbit")
                    ? "on Thunderbird 5"
                    : $"in {_locationDefinitionCatalog.GetByCode(bonusCondition.Location.Value).DisplayName}";
            }

            var description = bonusCondition.Location.HasValue
                ? $"{bonusCondition.Key.Value} (+{bonusCondition.Value}) (if {locationText})"
                : $"{bonusCondition.Key.Value} (+{bonusCondition.Value})";

            return new BonusConditionViewModel(
                Key: bonusCondition.Key.ToString(),
                Description: description
            );
        }

        private static RewardViewModel MapReward(ReferenceDisasterReward reward)
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
