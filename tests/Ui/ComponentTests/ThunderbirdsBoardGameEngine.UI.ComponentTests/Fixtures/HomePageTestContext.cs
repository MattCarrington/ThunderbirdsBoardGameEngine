using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public sealed class HomePageTestContext
    {
        public IGameService GameService { get; } = Substitute.For<IGameService>();

        public HomePageTestContext(BunitContext context)
        {
            context.Services.AddSingleton(GameService);
        }
    }
}