using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
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
                PerformingCharacter: new CharacterCode("scott"),
                PresentDisasterBonusKeys: [],
                PlayedFabCardCodes: []
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
                PerformingCharacter: new CharacterCode("scott"),
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("scott"),
                    new DisasterBonusKey("transmitter-truck")
                ],
                PlayedFabCardCodes: []
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result.TargetNumber);
            Assert.Equal(5, result.TotalBonus);
            Assert.Equal(2, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "scott" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "transmitter-truck" && b.Value == 3);
        }

        [Fact]
        public async Task RescueTargetAccountsForCharacterBonuses()
        {
            // Arrange
            var request = new CalculateRescueTargetQuery
            (
                DisasterCardCode: new CardCode("pit-of-peril"),
                PerformingCharacter: new CharacterCode("virgil"),
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("gordon")
                ],
                PlayedFabCardCodes: []
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
                PerformingCharacter: new CharacterCode("gordon"),
                PresentDisasterBonusKeys:
                [
                    new DisasterBonusKey("thunderbird-4"),
                    new DisasterBonusKey("virgil"),
                    new DisasterBonusKey("firefly")
                ],
                PlayedFabCardCodes:
                [
                    new CardCode("underwater-sealing-unit")
                ]
            );

            var mediator = CreateMediator();

            // Act
            var result = await mediator.Send(request, CancellationToken.None);

            // Assert
            Assert.Equal(-2, result.TargetNumber);
            Assert.Equal(13, result.TotalBonus);
            Assert.Equal(5, result.AppliedBonuses.Count);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "thunderbird-4" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "virgil" && b.Value == 2);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "firefly" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "gordon" && b.Value == 3);
            Assert.Contains(result.AppliedBonuses, b => b.Key == "underwater-sealing-unit" && b.Value == 3);
        }

        private static IMediator CreateMediator()
        {
            var source = CreateDisasterCatalog();
            var characters = CreateCharacterCatalog();

            var services = new ServiceCollection();
            services.AddSingleton<IDisasterDefinitionCatalog>(source);
            services.AddSingleton<ICharacterDefinitionCatalog>(characters);
            services.AddRules();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }

        private static FakeDisasterDefinitionCatalog CreateDisasterCatalog()
        {
            var sunProbe = new ReferenceDisasterDefinition(
                code: new CardCode("sun-probe"),
                displayName: "Sun Probe",
                difficultyNumber: 11,
                rescueType: RescueType.Space,
                location: new LocationCode("the-sun"),
                bonuses: [
                    new ReferenceDisasterBonus(new DisasterBonusKey("scott"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("virgil"), 2, new LocationCode("asia")),
                    new ReferenceDisasterBonus(new DisasterBonusKey("transmitter-truck"), 3, new LocationCode("asia"))
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Logistics)
                ]
            );
            var pitOfPeril = new ReferenceDisasterDefinition(
                code: new CardCode("pit-of-peril"),
                displayName: "Pit of Peril",
                difficultyNumber: 11,
                location: new LocationCode("africa"),
                rescueType: RescueType.Land,
                bonuses:
                [
                    new ReferenceDisasterBonus(new DisasterBonusKey("scott"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("mole"), 3, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("recovery-vehicles"), 2, null)
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Determination)
                ]
            );

            var terrorInNewYorkCity = new ReferenceDisasterDefinition(
                code: new CardCode("terror-in-new-york-city"),
                displayName: "Terror in New York City",
                difficultyNumber: 11,
                location: new LocationCode("north-america"),
                rescueType: RescueType.Sea,
                bonuses:
                [
                    new ReferenceDisasterBonus(new DisasterBonusKey("thunderbird-4"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("virgil"), 2, null),
                    new ReferenceDisasterBonus(new DisasterBonusKey("firefly"), 3, null)
                ],
                rewards:
                [
                    new ReferenceDisasterReward.PlayerChoice(),
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Teamwork)
                ]
            );

            return new FakeDisasterDefinitionCatalog(sunProbe, pitOfPeril, terrorInNewYorkCity);
        }

        private static FakeCharacterDefinitionCatalog CreateCharacterCatalog()
        {
            var scott = new ReferenceCharacterDefinition(
                code: new CharacterCode("scott"),
                displayName: "Scott",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Air,
                   value: 2
                )
            );
            var virgil = new ReferenceCharacterDefinition(
                code: new CharacterCode("virgil"),
                displayName: "Virgil",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Land,
                   value: 2
                )
            );
            var gordon = new ReferenceCharacterDefinition(
                code: new CharacterCode("gordon"),
                displayName: "Gordon",
                rescueBonus: new ReferenceCharacterRescueBonus(
                   rescueType: RescueType.Sea,
                   value: 3
                )
            );

            return new FakeCharacterDefinitionCatalog(scott, virgil, gordon);
        }
    }
}
