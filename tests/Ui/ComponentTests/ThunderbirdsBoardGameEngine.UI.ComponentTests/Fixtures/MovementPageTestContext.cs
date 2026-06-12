using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public sealed class MovementPageTestContext
    {
        public IThunderbirdMovementOptionsService ThunderbirdService { get; }
            = Substitute.For<IThunderbirdMovementOptionsService>();

        public IMovementLocationOptionsService LocationService { get; }
            = Substitute.For<IMovementLocationOptionsService>();

        public IMovementClientService MovementService { get; }
            = Substitute.For<IMovementClientService>();

        public MovementPageTestContext(BunitContext context)
        {
            context.Services.AddSingleton(ThunderbirdService);
            context.Services.AddSingleton(LocationService);
            context.Services.AddSingleton(MovementService);
        }
    }
}