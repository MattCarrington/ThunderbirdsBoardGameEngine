using Newtonsoft.Json.Linq;
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Helpers
{
    public class DisasterCardBuilder
    {
        private int _id = 1;
        private string _name = "Test Disaster";
        private int _difficultyNumber = 8;
        private BoardLocation _location = BoardLocation.NorthPacific;
        private RescueType _rescueType = RescueType.Sea;
        private readonly List<Bonus> _bonuses = new List<Bonus>();
        private readonly List<RewardOption> _rewardOptions = new List<RewardOption>();

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

        public DisasterCardBuilder WithBonus(Bonus bonus)
        {
            _bonuses.Add(bonus);
            return this;
        }

        public DisasterCardBuilder WithUserChoiceRewardOption()
        {
            _rewardOptions.Add(new RewardOption
            {
                IsUserChoice = true,
                
            });
            return this;
        }

        public DisasterCardBuilder WithSpecifiedReward(BonusToken token)
        {
            _rewardOptions.Add(new RewardOption
            {
                SpecifiedToken = token,
            });
            return this;
        }

        public DisasterCard Build()
        {
            if (_bonuses.Count == 0)
            {
                _bonuses.Add(new CharacterBonus { Character = Character.Scott, BonusValue = 1 });
            }

            if (_rewardOptions.Count == 0)
            {
                _rewardOptions.Add(new RewardOption
                {
                    IsUserChoice = true                    
                });
            }

            return new DisasterCard
            {
                Id = _id,
                Name = _name,
                Location = _location,
                RescueType = _rescueType,
                DifficultyNumber = _difficultyNumber,
                Bonuses = _bonuses,
                RewardOptions = _rewardOptions
            };
        }
    }
}
