using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Providers;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Providers
{
    public class CatalogRescueContextProviderTests
    {
        [Fact]
        public void GetRescueContext_WhenValidDisasterCard_ReturnsRescueContext()
        {
            // Arrange
            var characterBonus = new CharacterBonusCondition(Character.LadyPenelope, 2);
            var thunderbirdBonus = new ThunderbirdBonusCondition(ThunderbirdMachine.Thunderbird2, 1);
            var podVehicleBonus = new PodVehicleBonusCondition(PodVehicle.TransmitterTruck, 3);

            var disasterCard = new DisasterCardBuilder()
                .WithId(1)
                .WithName("Test Disaster Card")
                .WithDifficulty(7)
                .WithBonusCondition(characterBonus)
                .WithBonusCondition(thunderbirdBonus)
                .WithBonusCondition(podVehicleBonus)
                .Build();

            var catalog = Substitute.For<IDisasterCardCatalog>();
            catalog.GetById(disasterCard.Id).Returns(disasterCard);

            var provider = new CatalogRescueContextProvider(catalog);

            // Act
            var rescueContext = provider.GetRescueContext(disasterCard.Id);

            // Assert
            Assert.NotNull(rescueContext);
            Assert.Equal(7, rescueContext.DifficultyNumber);
            
            var expectedBonuses = new[]
            {
                new RescueBonus("ladypenelope", 2),
                new RescueBonus("thunderbird2", 1),
                new RescueBonus("transmittertruck", 3)
            };

            Assert.Equal(expectedBonuses.Length, rescueContext.Bonuses.Count);
            Assert.All(expectedBonuses, expected =>
                Assert.Contains(expected, rescueContext.Bonuses));
        }
    }
}
