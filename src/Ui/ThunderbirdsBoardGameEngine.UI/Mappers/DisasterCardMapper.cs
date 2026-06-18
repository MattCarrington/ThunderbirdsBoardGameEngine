using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Mappers
{
    public sealed class DisasterCardMapper
    {
        private readonly ILocationDefinitionCatalog _locationDefinitionCatalog;
        private readonly IDisasterBonusKeyDefinitionCatalog _disasterBonusKeyDefinitionCatalog;

        private static readonly LocationCode GeoStationaryOrbit = new("geo-stationary-orbit");

        public DisasterCardMapper(ILocationDefinitionCatalog locationDefinitionCatalog, IDisasterBonusKeyDefinitionCatalog disasterBonusKeyDefinitionCatalog)
        {
            _locationDefinitionCatalog = locationDefinitionCatalog;
            _disasterBonusKeyDefinitionCatalog = disasterBonusKeyDefinitionCatalog;
        }

        public DisasterCardViewModel Map(ReferenceDisasterDefinition disaster)
        {
            return new DisasterCardViewModel(
                Code: disaster.Code.ToString(),
                DisplayName: disaster.DisplayName,
                DifficultyNumber: disaster.DifficultyNumber,
                RescueType: disaster.RescueType.ToString(),
                Location: _locationDefinitionCatalog.TryGetByCode(disaster.Location, out var location) ? location.DisplayName : $"Unknown location '{disaster.Location}'",
                BonusConditions: disaster.Bonuses.Select(MapBonus).ToList(),
                Rewards: disaster.Rewards.Select(MapReward).ToList()
            );
        }

        private BonusConditionViewModel MapBonus(ReferenceDisasterBonus bonusCondition)
        {
            string? locationText = null;

            if (bonusCondition.Location.HasValue)
            {
                locationText = GetLocationDisplayText(bonusCondition.Location.Value);
            }

            var disasterBonusKeyDefinition = _disasterBonusKeyDefinitionCatalog.GetByCode(bonusCondition.Key);

            var description = bonusCondition.Location.HasValue
                ? $"{disasterBonusKeyDefinition.DisplayName} (+{bonusCondition.Value}) (if {locationText})"
                : $"{disasterBonusKeyDefinition.DisplayName} (+{bonusCondition.Value})";

            return new BonusConditionViewModel(
                Key: bonusCondition.Key.ToString(),
                Description: description
            );
        }

        private string GetLocationDisplayText(LocationCode locationCode)
        {
            if (locationCode == GeoStationaryOrbit)
            {
                return "on Thunderbird 5";
            }

            return _locationDefinitionCatalog.TryGetByCode(locationCode, out var location)
                ? $"in {location.DisplayName}"
                : $"in unknown location '{locationCode}'";
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
