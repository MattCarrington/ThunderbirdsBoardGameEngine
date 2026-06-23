using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Fixtures
{
    public sealed class DisasterCardPageTestContext
    {
        public IRescueClientService RescueClientService { get; }
            = Substitute.For<IRescueClientService>();

        public IDisasterCardService DisasterCardService { get; }
            = Substitute.For<IDisasterCardService>();

        public ICharacterService CharacterService { get; }
            = Substitute.For<ICharacterService>();

        public IRescueCalculationModifierService RescueCalculationModifierService { get; }
            = Substitute.For<IRescueCalculationModifierService>();

        public DisasterCardPageTestContext(BunitContext context)
        {
            context.Services.AddSingleton(RescueClientService);
            context.Services.AddSingleton(DisasterCardService);
            context.Services.AddSingleton(CharacterService);
            context.Services.AddSingleton(RescueCalculationModifierService);
        }
    }
}