using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Testing.Fixtures;
using ThunderbirdsBoardGameEngine.ReferenceData.Testing.TestData;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.DetermineFabCardBonus;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Rescue
{
    public class DetermineFabCardBonusTests
    {
        [Fact]
        public async Task ReturnsExpectedCardWhenDisasterExists()
        {
            // Arrange
            var request = new DetermineFabCardBonusQuery(TestDisasterCodes.SunProbe);

            var mediator = CreateMediator();

            // Act
            var response = await mediator.Send(request, TestContext.Current.CancellationToken);

            // Assert
            var expectedFabCard = new AstronautSpacewalk();

            Assert.Equal(expectedFabCard.CardCode, response.FabCardCode);
            Assert.Equal(expectedFabCard.BonusValue, response.BonusValue);
        }

        [Fact]
        public async Task ThrowsReferenceDataNotFoundExceptionWhenDisasterDoesNotExist()
        {
            // Arrange
            var request = new DetermineFabCardBonusQuery(new("NonExistentDisaster"));

            var mediator = CreateMediator();

            // Act & Assert
            await Assert.ThrowsAsync<ReferenceDataNotFoundException>(() => mediator.Send(request, TestContext.Current.CancellationToken));
        }

        private static IMediator CreateMediator()
        {
            var services = new ServiceCollection();

            services.AddRules();
            services.AddFakeCatalogs();

            var sp = services.BuildServiceProvider();
            return sp.GetRequiredService<IMediator>();
        }
    }
}
