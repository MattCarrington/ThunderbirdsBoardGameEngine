using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Rules.Application.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Providers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Rescue
{
    public class CalculateRescueTargetTests
    {
        [Fact]
        public async Task GivenACalculateRescueTargetTest_WhenSomethingHappens_ThenExpectedResultOccursAsync()
        {
            // Arrange
            var sunProbe = new DisasterCard(
                id: 1,
                name: "Sun Probe",
                code: "sun-probe",
                difficultyNumber: 11,
                location: BoardLocation.Sun,
                rescueType: RescueType.Space,
                bonusConditions:                [
                    new CharacterBonusCondition(Character.Scott, 2),
                    new CharacterBonusCondition(Character.Virgil, 2, BoardLocation.Asia),
                    new PodVehicleBonusCondition(PodVehicle.TransmitterTruck, 3, BoardLocation.Asia)
                ],
                rewardOptions:
                [
                    RewardOption.PlayerChoice(),
                    RewardOption.SpecifiedToken(BonusToken.Logistics)
                ]
            );

            var pitOfPeril = new DisasterCard(
                id: 2,
                name: "Pit of Peril",
                code: "pit-of-peril",
                difficultyNumber: 11,
                location: BoardLocation.Africa,
                rescueType: RescueType.Land,
                bonusConditions:
                [
                    new CharacterBonusCondition(Character.Scott, 2),
                    new PodVehicleBonusCondition(PodVehicle.Mole, 3),
                    new PodVehicleBonusCondition(PodVehicle.RecoveryVehicles, 2)
                ],
                rewardOptions:
                [
                    RewardOption.PlayerChoice(),
                    RewardOption.SpecifiedToken(BonusToken.Determination)
                ]
            );

            var terrorInNewYorkCity = new DisasterCard(
                id: 3,
                name: "Terror in New York City",
                code: "terror-in-new-york-city",
                difficultyNumber: 11,
                location: BoardLocation.NorthAmerica,
                rescueType: RescueType.Sea,
                bonusConditions:
                [
                    new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird4, 2),
                    new CharacterBonusCondition(Character.Virgil, 2),
                    new PodVehicleBonusCondition(PodVehicle.Firefly, 3)
                ],
                rewardOptions:
                [
                    RewardOption.PlayerChoice(),
                    RewardOption.SpecifiedToken(BonusToken.Teamwork)
                ]
            );

            var source = new FakeDisasterCardReferenceSource(sunProbe, pitOfPeril, terrorInNewYorkCity);

            var provider = new CatalogRescueProjectionProvider(source);

            var handler = new CalculateRescueTargetHandler(provider, new RescueTargetCalculator());

            var request = new CalculateRescueTargetQuery
            (
                DisasterCardId: 1,
                AppliedBonusKeys: ["scott", "transmittertruck"]
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result.TargetNumber);
            Assert.Equal(5, result.TotalBonus);
            Assert.Equal(2, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "scott" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "transmittertruck" && b.Value == 3);
        }
    }
}
