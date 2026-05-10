using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Mappers
{
    public sealed class DisasterCardMapper
    {
        private readonly ILocationDefinitionCatalog _locationDefinitionCatalog;
        private readonly IDisasterBonusKeyDefintionCatalog _disasterBonusKeyDefintionCatalog;

        private static readonly LocationCode GeoStationaryOrbit = new("geo-stationary-orbit");

        public DisasterCardMapper(ILocationDefinitionCatalog locationDefinitionCatalog, IDisasterBonusKeyDefintionCatalog disasterBonusKeyDefintionCatalog)
        {
            _locationDefinitionCatalog = locationDefinitionCatalog;
            _disasterBonusKeyDefintionCatalog = disasterBonusKeyDefintionCatalog;
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
                locationText = bonusCondition.Location.Value == GeoStationaryOrbit
                    ? "on Thunderbird 5"
                    : $"in {_locationDefinitionCatalog.GetByCode(bonusCondition.Location.Value).DisplayName}";
            }

            var disasterBonusKeyDefinition = _disasterBonusKeyDefintionCatalog.GetByCode(bonusCondition.Key);

            var description = bonusCondition.Location.HasValue
                ? $"{disasterBonusKeyDefinition.DisplayName} (+{bonusCondition.Value}) (if {locationText})"
                : $"{disasterBonusKeyDefinition.DisplayName} (+{bonusCondition.Value})";

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
