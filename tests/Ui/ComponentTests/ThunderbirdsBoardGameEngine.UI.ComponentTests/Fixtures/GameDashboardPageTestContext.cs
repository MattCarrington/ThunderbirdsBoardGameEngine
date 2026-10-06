using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public sealed class GameDashboardPageTestContext
    {
        public IGameService GameService { get; } = Substitute.For<IGameService>();

        public IMovementClientService MovementClientService { get; } = Substitute.For<IMovementClientService>();

        public GameDashboardPageTestContext(BunitContext context)
        {
            context.Services.AddSingleton(GameService);
            context.Services.AddSingleton(MovementClientService);
        }
    }
}