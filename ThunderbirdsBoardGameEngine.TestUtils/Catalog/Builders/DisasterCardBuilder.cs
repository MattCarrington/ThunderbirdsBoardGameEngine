using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders
{
    public class DisasterCardBuilder
    {
        private int _id = 1;
        private string _name = "Test Disaster";
        private string _code = "test-disaster";
        private int _difficultyNumber = 8;
        private BoardLocation _location = BoardLocation.NorthPacific;
        private RescueType _rescueType = RescueType.Sea;
        private readonly List<BonusCondition> _bonuses = [];
        private readonly List<RewardOption> _rewardOptions = [];
        
        public DisasterCardBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public DisasterCardBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public DisasterCardBuilder WithCode(string code)
        {
            _code = code;
            return this;
        }

        public DisasterCardBuilder WithLocation(BoardLocation location)
        {
            _location = location;
            return this;
        }

        public DisasterCardBuilder WithRescueType(RescueType rescueType)
        {
            _rescueType = rescueType;
            return this;
        }

        public DisasterCardBuilder WithDifficulty(int difficulty)
        {
            _difficultyNumber = difficulty;
            return this;
        }

        public DisasterCardBuilder WithBonusCondition(BonusCondition bonus)
        {
            _bonuses.Add(bonus);
            return this;
        }

        public DisasterCardBuilder WithUserChoiceRewardOption()
        {
            _rewardOptions.Add(RewardOption.PlayerChoice());
            return this;
        }

        public DisasterCardBuilder WithSpecifiedReward(BonusToken token)
        {
            _rewardOptions.Add(RewardOption.SpecifiedToken(token));
            return this;
        }

        public DisasterCard Build()
        {
            if (_bonuses.Count == 0)
            {
                _bonuses.Add(new CharacterBonusCondition(Character.Scott, 1));
            }

            if (_rewardOptions.Count == 0)
            {
                _rewardOptions.Add(RewardOption.PlayerChoice());
            }

            return new DisasterCard(_id, _name, _code, _difficultyNumber, _location, _rescueType, _bonuses, _rewardOptions);

        }
    }
}
