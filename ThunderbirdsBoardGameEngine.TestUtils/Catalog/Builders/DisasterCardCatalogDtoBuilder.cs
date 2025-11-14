using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders
{
    public sealed class DisasterCardCatalogDtoBuilder
    {
        // Backing fields
        private int _id = 1;
        private string _name = "Test Disaster";
        private int _difficultyNumber = 8;
        private string _location = "NorthPacific";
        private string _rescueType = "Sea";
        private readonly List<BonusConditionCatalogDto> _bonuses = new();
        private readonly List<RewardOptionCatalogDto> _rewards = new();
        private string _code = "test-disaster";

        // Behavior flags
        private bool _ensureDefaults = true;
        private bool _forceEmptyBonuses;
        private bool _forceEmptyRewards;

        // Fluent setters
        public DisasterCardCatalogDtoBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithCode(string code)
        {
            _code = code;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithDifficulty(int value)
        {
            _difficultyNumber = value;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithLocation(string location)
        {
            _location = location;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithRescueType(string rescueType)
        {
            _rescueType = rescueType;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithBonusCondition(BonusConditionCatalogDto bonus)
        {
            _bonuses.Add(bonus);
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithBonuses(IEnumerable<BonusConditionCatalogDto> bonuses)
        {
            _bonuses.AddRange(bonuses);
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithRewardOption(RewardOptionCatalogDto reward)
        {
            _rewards.Add(reward);
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithRewards(IEnumerable<RewardOptionCatalogDto> rewards)
        {
            _rewards.AddRange(rewards);
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithPlayerChoiceReward()
        {
            _rewards.Add(new PlayerChoiceRewardCatalogDto());
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithSpecifiedRewardToken(string token)
        {
            _rewards.Add(new TokenRewardCatalogDto { Token = token });
            return this;
        }

        // Test helpers / opt-out
        public DisasterCardCatalogDtoBuilder NoDefaults()
        {
            _ensureDefaults = false;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithEmptyBonuses()
        {
            _forceEmptyBonuses = true;
            return this;
        }

        public DisasterCardCatalogDtoBuilder WithEmptyRewards()
        {
            _forceEmptyRewards = true;
            return this;
        }

        public DisasterCardCatalogDto Build()
        {
            // 1) Apply explicit test overrides into local lists (don’t mutate builder state)
            var bonuses =
                _forceEmptyBonuses ? new List<BonusConditionCatalogDto>() : new List<BonusConditionCatalogDto>(_bonuses);

            var rewards =
                _forceEmptyRewards ? new List<RewardOptionCatalogDto>() : new List<RewardOptionCatalogDto>(_rewards);

            // 2) Safe defaults for happy paths (unless disabled)
            if (_ensureDefaults)
            {
                if (bonuses.Count == 0 && !_forceEmptyBonuses)
                {
                    bonuses.Add(new CharacterBonusCatalogDto
                    {
                        Character = "Virgil",
                        BonusValue = 1
                    });
                }

                if (rewards.Count == 0 && !_forceEmptyRewards)
                {
                    rewards.Add(new PlayerChoiceRewardCatalogDto());
                }
            }

            // 3) Return copies to avoid aliasing across tests
            return new DisasterCardCatalogDto
            {
                Id = _id,
                Name = _name,
                Code = _code,
                DifficultyNumber = _difficultyNumber,
                Location = _location,
                RescueType = _rescueType,
                BonusConditions = bonuses,
                RewardOptions = rewards
            };
        }
    }
}