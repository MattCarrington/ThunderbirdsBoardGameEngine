using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.Assertions
{
    public static class DisasterCardDtoAssertions
    {
        public static void AssertDisasterCardDtoEqual(DisasterCardDto expected, DisasterCardDto actual)
        {
            // Assert DisasterCardDto properties
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.DifficultyNumber, actual.DifficultyNumber);
            Assert.Equal(expected.Location, actual.Location);
            Assert.Equal(expected.RescueType, actual.RescueType);

            // Assert BonusConditionDto collection
            Assert.Equal(expected.BonusConditions.Count, actual.BonusConditions.Count);
            Assert.All(actual.BonusConditions, bc =>
                Assert.False(string.IsNullOrWhiteSpace(bc.Description)));

            // Assert RewardDto collection
            Assert.Equal(expected.Rewards.Count, actual.Rewards.Count);
            Assert.All(actual.Rewards, r =>
                Assert.False(string.IsNullOrWhiteSpace(r.DisplayName)));
        }
    }
}
