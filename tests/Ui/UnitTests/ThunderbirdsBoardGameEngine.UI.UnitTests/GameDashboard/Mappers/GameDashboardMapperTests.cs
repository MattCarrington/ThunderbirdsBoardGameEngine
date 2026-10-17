using NSubstitute;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Mappers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.GameDashboard.Mappers
{
    public class GameDashboardMapperTests
    {
        [Fact]
        public void ToViewModel_ValidDto_ShouldResolveNames()
        {
            // Arrange
            var thunderbirdCode = new ThunderbirdCode("tb1");
            var thunderbird = CreateThunderbird(thunderbirdCode.Value, "Thunderbird 1");

            var locationCode = new LocationCode("loc1");
            var location = CreateLocation(locationCode.Value, "Location 1");

            var characterCode = new CharacterCode("char1");
            var character = CreateCharacter(characterCode.Value, "Character 1");

            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            thunderbirdCatalog.GetByCode(thunderbirdCode).Returns(thunderbird);

            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.GetByCode(locationCode).Returns(location);

            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();
            characterCatalog.GetByCode(characterCode).Returns(character);

            var mapper = new GameDashboardMapper(thunderbirdCatalog, locationCatalog, characterCatalog);

            var dto = new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines =
                [
                    new ThunderbirdMachineStateDto
                    {
                        ThunderbirdCode = thunderbirdCode.Value,
                        LocationCode = locationCode.Value,
                        OccupantCharacterCodes = [ characterCode.Value ]
                    }
                ]
            };

            // Act
            var result = mapper.ToViewModel(dto);

            // Assert
            var thunderbirdMachine = Assert.Single(result.ThunderbirdMachines);
            Assert.Equal(thunderbird.DisplayName, thunderbirdMachine.ThunderbirdDisplayName);
            Assert.Equal(location.DisplayName, thunderbirdMachine.LocationDisplayName);

            var occupant = Assert.Single(thunderbirdMachine.Occupants);
            Assert.Equal(character.DisplayName, occupant.CharacterDisplayName);
        }

        [Fact]
        public void ToViewModel_WhenOccupantsEmpty_EmptyOccupants()
        {
            // Arrange
            var dto = new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines =
                [
                    new ThunderbirdMachineStateDto
                    {
                        ThunderbirdCode = "tb1",
                        LocationCode = "loc1",
                        OccupantCharacterCodes = []
                    }
                ]
            };

            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            thunderbirdCatalog.GetByCode(Arg.Any<ThunderbirdCode>()).Returns(CreateThunderbird("tb1", "Thunderbird 1"));

            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.GetByCode(Arg.Any<LocationCode>()).Returns(CreateLocation("loc1", "Location 1"));

            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();

            var mapper = new GameDashboardMapper(thunderbirdCatalog, locationCatalog, characterCatalog);

            // Act
            var result = mapper.ToViewModel(dto);

            // Assert
            var thunderbirdMachine = Assert.Single(result.ThunderbirdMachines);
            Assert.Empty(thunderbirdMachine.Occupants);

            characterCatalog.DidNotReceive().GetByCode(Arg.Any<CharacterCode>());
        }

        [Fact]
        public void ToViewModel_WhenNoThunderbirds_EmptyThunderbirds()
        {
            // Arrange
            var dto = new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines = []
            };

            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();

            var mapper = new GameDashboardMapper(thunderbirdCatalog, locationCatalog, characterCatalog);

            // Act
            var result = mapper.ToViewModel(dto);

            // Assert
            Assert.Empty(result.ThunderbirdMachines);

            thunderbirdCatalog.DidNotReceive().GetByCode(Arg.Any<ThunderbirdCode>());
            locationCatalog.DidNotReceive().GetByCode(Arg.Any<LocationCode>());
        }

        [Fact]
        public void ToViewModel_WhenMultipleThunderbirds_ShouldResolveAllLists()
        {
            // Arrange
            var thunderbird1 = CreateThunderbird("tb1", "Thunderbird 1");
            var thunderbird2 = CreateThunderbird("tb2", "Thunderbird 2");

            var location1 = CreateLocation("loc1", "Location 1");
            var location2 = CreateLocation("loc2", "Location 2");

            var character1 = CreateCharacter("char1", "Character 1");
            var character2 = CreateCharacter("char2", "Character 2");
            var character3 = CreateCharacter("char3", "Character 3");

            var thunderbirdCatalog = Substitute.For<IThunderbirdDefinitionCatalog>();
            thunderbirdCatalog.GetByCode(new ThunderbirdCode(thunderbird1.Code.Value)).Returns(thunderbird1);
            thunderbirdCatalog.GetByCode(new ThunderbirdCode(thunderbird2.Code.Value)).Returns(thunderbird2);

            var locationCatalog = Substitute.For<ILocationDefinitionCatalog>();
            locationCatalog.GetByCode(new LocationCode(location1.Code.Value)).Returns(location1);
            locationCatalog.GetByCode(new LocationCode(location2.Code.Value)).Returns(location2);

            var characterCatalog = Substitute.For<ICharacterDefinitionCatalog>();
            characterCatalog.GetByCode(new CharacterCode(character1.Code.Value)).Returns(character1);
            characterCatalog.GetByCode(new CharacterCode(character2.Code.Value)).Returns(character2);
            characterCatalog.GetByCode(new CharacterCode(character3.Code.Value)).Returns(character3);

            var mapper = new GameDashboardMapper(thunderbirdCatalog, locationCatalog, characterCatalog);

            var dto = new GameStateResponseDto
            {
                GameId = Guid.NewGuid(),
                ThunderbirdMachines =
                [
                    new ThunderbirdMachineStateDto
                    {
                        ThunderbirdCode = thunderbird1.Code.Value,
                        LocationCode = location1.Code.Value,
                        OccupantCharacterCodes = [ character1.Code.Value, character3.Code.Value ]
                    },
                    new ThunderbirdMachineStateDto
                    {
                        ThunderbirdCode = thunderbird2.Code.Value,
                        LocationCode = location2.Code.Value,
                        OccupantCharacterCodes = [ character2.Code.Value ]
                    }
                ]
            };

            // Act
            var result = mapper.ToViewModel(dto);

            // Assert
            Assert.Equal(2, result.ThunderbirdMachines.Count);
            var tbMachine1 = result.ThunderbirdMachines.First(tm => tm.ThunderbirdCode == thunderbird1.Code.Value);

            Assert.Equal(2, tbMachine1.Occupants.Count);
        }

        private static ReferenceThunderbirdDefinition CreateThunderbird(string code, string displayName)
        {
            return new(
                new ThunderbirdCode(code),
                displayName,
                MovementDomain.Earth,
                topSpeed: 1);
        }

        private static ReferenceLocationDefinition CreateLocation(string code, string displayName)
        {
            return new(
                new LocationCode(code),
                displayName,
                MovementDomain.Earth);
        }

        private static ReferenceCharacterDefinition CreateCharacter(string code, string displayName)
        {
            return new(
                new CharacterCode(code),
                displayName,
                rescueBonus: null);
        }
    }
}
