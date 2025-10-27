using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.Services;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Services
{
    public class DisasterCardServiceTests
    {
        [Fact]
        public void GetAll_WhenCardsExist_ReturnsCatalogCards()
        {
            // Arrange
            var disasterCards = ImmutableArray.Create(            
                new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithDifficulty(7).WithSpecifiedReward(BonusToken.Intelligence).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithDifficulty(8).WithLocation(BoardLocation.Asia).WithUserChoiceRewardOption().Build()
            );

            var catalog = Substitute.For<IDisasterCardCatalog>();
            catalog.Cards.Returns(disasterCards);

            var service = new DisasterCardService(catalog);

            // Act
            var result = service.GetAll();

            // Assert
            Assert.IsType<ImmutableArray<DisasterCard>>(result);
            Assert.Equal(disasterCards, result);

            _ = catalog.Received(1).Cards;
        }
    }
}
