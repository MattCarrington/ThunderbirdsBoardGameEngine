using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.DisasterCards.Services
{
    public class DisasterCardServiceTests
    {
        [Fact]
        public void GetAll_WhenCalled_CallsCatalog()
        {
            // Arrange
            var disaster = CreateMinimalDisaster("DC001");
            var disasterCatalog = Substitute.For<IDisasterDefinitionCatalog>();
            disasterCatalog.GetAll().Returns(new[] { disaster }.ToImmutableArray());

            var mapper = CreateMapper();

            var service = new DisasterCardService(disasterCatalog, mapper);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Single(result);
            disasterCatalog.Received(1).GetAll();
        }

        [Fact]
        public void GetAll_WhenCatalogReturnsMultipleItems_ReturnsCorrectCount()
        {
            // Arrange
            var disaster1 = CreateMinimalDisaster("DC001");
            var disaster2 = CreateMinimalDisaster("DC002");
            var disaster3 = CreateMinimalDisaster("DC003");

            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetAll().Returns(new[] { disaster1, disaster2, disaster3 }.ToImmutableArray());

            var mapper = CreateMapper();

            var service = new DisasterCardService(catalog, mapper);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public void GetAll_WhenCatalogReturnsItemsInAnyOrder_ReturnsItemsSortedByDisplayName()
        {
            // Arrange
            var disaster1 = new ReferenceDisasterDefinition(
                code: new CardCode("DC003"),
                displayName: "Space Disaster",
                difficultyNumber: 5,
                location: new LocationCode("london"),
                rescueType: RescueType.Space,
                bonuses: [new ReferenceDisasterBonus(new DisasterBonusKey("test"), 1, null)],
                rewards: [new ReferenceDisasterReward.SpecificToken(BonusToken.Teamwork)]
            );
            var disaster2 = new ReferenceDisasterDefinition(
                code: new CardCode("DC002"),
                displayName: "Land Disaster",
                difficultyNumber: 3,
                location: new LocationCode("paris"),
                rescueType: RescueType.Land,
                bonuses: [new ReferenceDisasterBonus(new DisasterBonusKey("test2"), 2, null)],
                rewards: [new ReferenceDisasterReward.SpecificToken(BonusToken.Intelligence)]
            );
            var disaster3 = new ReferenceDisasterDefinition(
                code: new CardCode("DC001"),
                displayName: "Air Disaster",
                difficultyNumber: 4,
                location: new LocationCode("newyork"),
                rescueType: RescueType.Air,
                bonuses: [new ReferenceDisasterBonus(new DisasterBonusKey("test3"), 3, null)],
                rewards: [new ReferenceDisasterReward.SpecificToken(BonusToken.Determination)]
            );

            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetAll().Returns(new[] { disaster1, disaster2, disaster3 }.ToImmutableArray());

            var mapper = CreateMapper();

            var service = new DisasterCardService(catalog, mapper);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("DC001", result[0].Code);
            Assert.Equal("DC002", result[1].Code);
            Assert.Equal("DC003", result[2].Code);
        }

        [Fact]
        public void GetAll_WhenCatalogIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetAll().Returns(ImmutableArray<ReferenceDisasterDefinition>.Empty);

            var mapper = CreateMapper();

            var service = new DisasterCardService(catalog, mapper);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetByCode_WhenDisasterExists_CallsCatalogWithCorrectCode()
        {
            // Arrange
            var disaster = CreateMinimalDisaster("DC123");
            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<CardCode>()).Returns(disaster);

            var mapper = CreateMapper();

            var service = new DisasterCardService(catalog, mapper);

            // Act
            service.GetByCode("DC123");

            // Assert
            catalog.Received(1).GetByCode(Arg.Is<CardCode>(code => code.ToString() == "DC123"));
        }

        [Fact]
        public void GetByCode_WhenCodeExists_ReturnsNonNull()
        {
            // Arrange
            var disaster = CreateMinimalDisaster("DC001");
            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetByCode(new CardCode("DC001")).Returns(disaster);

            var mapper = CreateMapper();

            var service = new DisasterCardService(catalog, mapper);

            // Act
            var result = service.GetByCode("DC001");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetByCode_WhenCalledPreservesCodeValue()
        {
            // Arrange
            var disaster = CreateMinimalDisaster("DC456");
            var catalog = Substitute.For<IDisasterDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<CardCode>()).Returns(disaster);

            var mapper = CreateMapper();

            var service = new DisasterCardService(catalog, mapper);

            // Act
            var result = service.GetByCode("DC456");

            // Assert
            Assert.Equal("DC456", result.Code);
        }

        private static ReferenceDisasterDefinition CreateMinimalDisaster(string code)
        {
            var bonus = new ReferenceDisasterBonus(
                new DisasterBonusKey("test"),
                value: 1,
                location: null
            );

            var reward = new ReferenceDisasterReward.SpecificToken(
                BonusToken.Teamwork
            );

            return new ReferenceDisasterDefinition(
                code: new CardCode(code),
                displayName: "Test Disaster",
                difficultyNumber: 5,
                location: new LocationCode("london"),
                rescueType: RescueType.Air,
                bonuses: new[] { bonus },
                rewards: new[] { reward }
            );
        }

        private static DisasterCardMapper CreateMapper()
        {
            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.GetByCode(Arg.Any<LocationCode>())
                .Returns(new ReferenceLocationDefinition(new LocationCode("london"), "London", MovementDomain.Earth));

            var disasterBonusKeyCatalog = Substitute.For<IDisasterBonusKeyDefinitionCatalog>();
            disasterBonusKeyCatalog.GetByCode(Arg.Any<DisasterBonusKey>())
                .Returns(new DisasterBonusKeyDefinition(new DisasterBonusKey("test"), "Test Bonus"));

            return new DisasterCardMapper(locationCatalog, disasterBonusKeyCatalog);
        }
    }
}
