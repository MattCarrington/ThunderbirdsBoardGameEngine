using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public sealed class HomePageTestContext
    {
        public IGameService GameService { get; } = Substitute.For<IGameService>();

        public ICharacterService CharacterService { get; } = Substitute.For<ICharacterService>();

        public HomePageTestContext(BunitContext context)
        {
            context.Services.AddSingleton(GameService);
            context.Services.AddSingleton(CharacterService);
        }
    }
}