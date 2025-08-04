using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Pages;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Pages
{
    public class DisasterCardsTests : TestContext
    {
        [Fact]
        public void RendersDropdownWithCards()
        {
            // Arrange
            var mockService = Substitute.For<IDisasterCardService>();
            var cards = new List<DisasterCardDto>
            {
                new() { Id = 1, Name = "Card One" },
                new() { Id = 2, Name = "Card Two" }
            };

            mockService.GetAllAsync().Returns(cards);
            Services.AddSingleton(mockService);

            // Act
            var cut = RenderComponent<DisasterCards>();

            // Assert
            var options = cut.FindAll("option");
            Assert.Contains(options, o => o.TextContent == "Card One");
            Assert.Contains(options, o => o.TextContent == "Card Two");
        }

        [Fact]
        public void OnSelectDisplayCard()
        {
            throw new NotImplementedException();
        }
    }
}
