using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public sealed class GameDashboardPageTestContext
    {
        public IGameService GameService { get; } = Substitute.For<IGameService>();

        public GameDashboardPageTestContext(BunitContext context)
        {
            context.Services.AddSingleton(GameService);
        }
    }
}