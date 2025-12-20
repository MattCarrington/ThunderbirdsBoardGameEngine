using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Application.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Rescue
{
    public class CalculateRescueTargetTests
    {
        [Fact]
        public async Task GivenCardAndBonuses_WhenCalculatingRescueTarget_ReturnsExpectedTargetRoll()
        {
            // Arrange
            var sunProbe = new DisasterCard(
                id: 1,
                name: "Sun Probe",
                code: "sun-probe",
                difficultyNumber: 11,
                location: BoardLocation.Sun,
                rescueType: RescueType.Space,
                bonusConditions: [
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

            var request = new CalculateRescueTargetQuery
            (
                DisasterCardId: 1,
                AppliedBonusKeys: [new DisasterBonusKey("character:scott"), new DisasterBonusKey("podvehicle:transmittertruck")]
            );

            var services = new ServiceCollection();
            services.AddSingleton<IDisasterCardReferenceSource>(source);
            services.AddRules();

            using var sp = services.BuildServiceProvider();

            var mediator = sp.GetRequiredService<IMediator>();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result.TargetNumber);
            Assert.Equal(5, result.TotalBonus);
            Assert.Equal(2, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == new DisasterBonusKey("character:scott") && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == new DisasterBonusKey("podvehicle:transmittertruck") && b.Value == 3);
        }
    }
}
