using NSubstitute;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.DisasterCards.Mappers
{
    public class DisasterCardMapperTests
    {
        [Fact]
        public void Map_WhenDisasterDefinitionIsMapped_MapsCoreFields()
        {
            // Arrange
            var disaster = CreateDisasterDefinition(
                bonuses: new[]
                {
                    new ReferenceDisasterBonus(
                        key: new DisasterBonusKey("bonus1"),
                        value: 2,
                        location: null)
                },
                rewards: new[]
                {
                    new ReferenceDisasterReward.PlayerChoice()
                }
            );

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(disaster);

            // Assert
            Assert.IsType<DisasterCardViewModel>(result);
            Assert.Equal(disaster.DisplayName, result.DisplayName);
            Assert.Equal(disaster.DifficultyNumber, result.DifficultyNumber);
            Assert.Equal("Test Location", result.Location);
            Assert.Equal("Air", result.RescueType);
            Assert.NotEmpty(result.BonusConditions);
            Assert.NotEmpty(result.Rewards);
        }

        [Fact]
        public void Map_WhenBonusHasNoLocation_MapsBonusDescriptionWithoutLocation()
        {
            var disaster = CreateDisasterDefinition(
                bonuses: new[]
                {
                    new ReferenceDisasterBonus(
                        key: new DisasterBonusKey("bonus1"),
                        value: 2,
                        location: null)
                },
                rewards: new[]
                {
                    new ReferenceDisasterReward.PlayerChoice()
                }
            );

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(disaster);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);
            Assert.Equal("Bonus 1 (+2)", bonus.Description);
        }

        [Fact]
        public void Map_WhenBonusHasLocation_MapsBonusDescriptionWithLocation()
        {
            var disaster = CreateDisasterDefinition(
                bonuses: new[]
                {
                new ReferenceDisasterBonus(
                    key: new DisasterBonusKey("bonus1"),
                    value: 2,
                    location: new LocationCode("test-location"))
                },
                rewards: new[]
                {
                new ReferenceDisasterReward.PlayerChoice()
                }
            );

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(disaster);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);
            Assert.Equal("Bonus 1 (+2) (if in Test Location)", bonus.Description);
        }

        [Fact]
        public void Map_WhenBonusLocationIsGeoStationaryOrbit_MapsLocationAsThunderbird5()
        {
            var disaster = CreateDisasterDefinition(
                bonuses: new[]
                {
                new ReferenceDisasterBonus(
                    key: new DisasterBonusKey("bonus1"),
                    value: 2,
                    location: new LocationCode("geo-stationary-orbit"))
                },
                rewards: new[]
                {
                new ReferenceDisasterReward.PlayerChoice()
                }
            );

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(disaster);

            // Assert
            var bonus = Assert.Single(result.BonusConditions);
            Assert.Equal("Bonus 1 (+2) (if on Thunderbird 5)", bonus.Description);
        }


        [Fact]
        public void Map_WhenRewardIsPlayerChoice_MapsDisplayNameAsPlayerChoice()
        {
            var disaster = CreateDisasterDefinition(
                bonuses: new[]
                {
                new ReferenceDisasterBonus(
                    key: new DisasterBonusKey("bonus1"),
                    value: 2,
                    location: new LocationCode("test-location"))
                },
                rewards: new[]
                {
                new ReferenceDisasterReward.PlayerChoice()
                }
            );

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(disaster);

            // Assert
            var reward = Assert.Single(result.Rewards);
            Assert.Equal("Player Choice", reward.Description);
        }

        [Fact]
        public void Map_WhenRewardIsSpecificToken_MapsTokenDisplayName()
        {
            var disaster = CreateDisasterDefinition(
                bonuses: new[]
                {
                    new ReferenceDisasterBonus(
                        key: new DisasterBonusKey("bonus1"),
                        value: 2,
                        location: new LocationCode("test-location"))
                },
                rewards: new[]
                    {
                    new ReferenceDisasterReward.SpecificToken(BonusToken.Intelligence)
                }
            );

            var mapper = CreateMapper();

            // Act
            var result = mapper.Map(disaster);

            // Assert
            var reward = Assert.Single(result.Rewards);
            Assert.Equal("Intelligence", reward.Description);
        }

        [Fact]
        public void Map_WhenLocationIsGeoStationaryOrbit_MapsLocationDisplayName()
        {
            // Arrange
            var geoStationaryOrbit = new ReferenceLocationDefinition(
                code: new LocationCode("geo-stationary-orbit"),
                displayName: "Geo-Stationary Orbit",
                domain: MovementDomain.Space
            );

            var disaster = new ReferenceDisasterDefinition(
                code: new CardCode("test-disaster"),
                displayName: "Test Disaster",
                difficultyNumber: 3,
                location: geoStationaryOrbit.Code,
                rescueType: RescueType.Air,
                bonuses: new[]
                {
                    new ReferenceDisasterBonus(
                        key: new DisasterBonusKey("bonus1"),
                        value: 2,
                        location: null)
                },
                rewards: new[]
                {
                    new ReferenceDisasterReward.PlayerChoice()
                }
            );

            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.TryGetByCode(Arg.Is(geoStationaryOrbit.Code), out Arg.Any<ReferenceLocationDefinition>()).Returns(callInfo =>
            {
                callInfo[1] = geoStationaryOrbit;
                return true;
            });

            var disasterBonusKeyCatalog = Substitute.For<IDisasterBonusKeyDefinitionCatalog>();
            disasterBonusKeyCatalog.GetByCode(Arg.Any<DisasterBonusKey>()).Returns(new DisasterBonusKeyDefinition(
                Key: new DisasterBonusKey("bonus1"),
                DisplayName: "Bonus 1"
            ));

            var mapper = new DisasterCardMapper(locationCatalog, disasterBonusKeyCatalog);

            // Act
            var result = mapper.Map(disaster);

            // Assert
            Assert.Equal(geoStationaryOrbit.DisplayName, result.Location);
        }

        private static ReferenceDisasterDefinition CreateDisasterDefinition(ReferenceDisasterBonus[] bonuses, ReferenceDisasterReward[] rewards)
        {
            return new ReferenceDisasterDefinition(
                code: new CardCode("test-disaster"),
                displayName: "Test Disaster",
                difficultyNumber: 3,
                location: new LocationCode("test-location"),
                rescueType: RescueType.Air,
                bonuses: bonuses,
                rewards: rewards
            );
        }

        private static DisasterCardMapper CreateMapper()
        {
            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.TryGetByCode(Arg.Any<LocationCode>(), out Arg.Any<ReferenceLocationDefinition>()).Returns(callInfo =>
            {
                callInfo[1] = new ReferenceLocationDefinition(
                    code: new LocationCode("test-location"),
                    displayName: "Test Location",
                    domain: MovementDomain.Earth
                );
                return true;
            });

            var disasterBonusKeyCatalog = Substitute.For<IDisasterBonusKeyDefinitionCatalog>();
            disasterBonusKeyCatalog.GetByCode(Arg.Any<DisasterBonusKey>()).Returns(new DisasterBonusKeyDefinition(
                Key: new DisasterBonusKey("bonus1"),
                DisplayName: "Bonus 1"
            ));

            return new DisasterCardMapper(locationCatalog, disasterBonusKeyCatalog);
        }
    }
}
