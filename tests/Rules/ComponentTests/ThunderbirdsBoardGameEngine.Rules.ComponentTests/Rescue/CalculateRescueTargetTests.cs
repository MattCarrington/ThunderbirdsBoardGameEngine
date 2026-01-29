using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Rescue
{
    public class CalculateRescueTargetTests
    {
        [Fact]
        public async Task RescueTargetHandlesNoBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode("pit-of-peril"),
                PerformingCharacter: CharacterCode.Scott,
                PresentDisasterBonusKeys: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(11, result.TargetNumber);
            Assert.Equal(0, result.TotalBonus);
            Assert.Empty(result.AppliedBonuses);
        }

        [Fact]
        public async Task RescueTargetAccountsForDisasterBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode("sun-probe"),
                PerformingCharacter: CharacterCode.Scott,
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("character:scott"),
                    new DisasterBonusKey("podvehicle:transmittertruck")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result.TargetNumber);
            Assert.Equal(5, result.TotalBonus);
            Assert.Equal(2, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "character:scott" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "podvehicle:transmittertruck" && b.Value == 3);
        }

        [Fact]
        public async Task RescueTargetAccountsForCharacterBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode("pit-of-peril"),
                PerformingCharacter: CharacterCode.Virgil,
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("character:gordon")
                ]
            );

            var mediator = CreateMediator();
            // Act
            var result = await mediator.Send(request, CancellationToken.None);
            // Assert
            Assert.Equal(9, result.TargetNumber);
            Assert.Equal(2, result.TotalBonus);
            Assert.Single(result.AppliedBonuses);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "virgil" && b.Value == 2);
        }

        [Fact]
        public async Task RescueTargetAccountsAllBonuses()

        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode("terror-in-new-york-city"),
                PerformingCharacter: CharacterCode.Gordon,
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("thunderbird:thunderbird4"),
                    new DisasterBonusKey("character:virgil"),
                    new DisasterBonusKey("podvehicle:firefly")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(1, result.TargetNumber);
            Assert.Equal(10, result.TotalBonus);
            Assert.Equal(4, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "thunderbird:thunderbird4" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "character:virgil" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "podvehicle:firefly" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "gordon" && b.Value == 3);
        }

        private static IMediator CreateMediator()
        {
            var source = CreateDisasterReferenceSource();
            var characters = new FakeCharacterDefinitionReferenceSource();

            var services = new ServiceCollection();
            services.AddSingleton<IDisasterCardReferenceSource>(source);
            services.AddSingleton<ICharacterDefinitionReferenceSource>(characters);
            services.AddRules();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }

        private static FakeDisasterCardReferenceSource CreateDisasterReferenceSource()
        {
            var sunProbe = new DisasterCard(
                            id: 1,
                            name: "Sun Probe",
                            code: new CardCode("sun-probe"),
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
                code: new CardCode("pit-of-peril"),
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
                code: new CardCode("terror-in-new-york-city"),
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
            return source;
        }
    }
}
