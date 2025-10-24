using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Api.Startup;
using ThunderbirdsBoardGameEngine.Catalog.Application.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Startup
{
    public class DisasterCardWarmupHostedServiceTests
    {
        private static readonly ImmutableArray<DisasterCard> _cards =
        [
            new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithDifficulty(5).WithLocation(BoardLocation.Asia).WithSpecifiedReward(BonusToken.Teamwork).Build(),
            new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithDifficulty(8).WithLocation(BoardLocation.NorthAmerica).WithUserChoiceRewardOption().Build()
        ];

        [Fact]
        public async Task StartAsync_WhenNotEnabled_ShouldNotCallDisasterCardService()
        {
            // Arrange
            var options = new DisasterCardWarmupOptions
            {
                Enabled = false
            };

            var service = CreateService(_cards);

            var warmup = CreateWarmupService(options, service);

            // Act
            await warmup.StartAsync(CancellationToken.None);

            // Assert
            await service.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StartAsync_WhenCalled_ShouldCallDisasterCardService()
        {
            // Arrange
            var service = CreateService(_cards);

            var warmup = CreateWarmupService(service);

            // Act
            await warmup.StartAsync(CancellationToken.None);

            // Assert
            await service.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StartAsync_WhenServiceThrows_Rethrows()
        {
            // Arrange
            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync(Arg.Any<CancellationToken>())
                  .Returns<Task<IReadOnlyList<DisasterCard>>>(_ => throw new InvalidOperationException("Test exception"));

            var warmup = CreateWarmupService(service);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => warmup.StartAsync(CancellationToken.None));
        }

        [Fact]
        public async Task StartAsync_WhenServiceNeverCompletes_ShouldTimeout()
        {
            // Arrange
            var options = new DisasterCardWarmupOptions
            {
                Enabled = true,
                Timeout = TimeSpan.FromMilliseconds(100)
            };

            var service = CreateService(_cards, Timeout.Infinite);

            var warmup = CreateWarmupService(options, service);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => warmup.StartAsync(CancellationToken.None));
        }

        [Fact]
        public async Task StartAsync_WhenCancellationTokenCancelled_ShouldContinue()
        {
            // Arrange
            CancellationToken observed = default;

            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync(Arg.Any<CancellationToken>())
               .Returns(callinfo => 
               { 
                   observed = callinfo.Arg<CancellationToken>(); 
                   return Task.FromResult<IReadOnlyList<DisasterCard>>(Array.Empty<DisasterCard>()); });

            var warmup = CreateWarmupService(service);

            using var hostToken = new CancellationTokenSource();
            await hostToken.CancelAsync();

            // Act
            try 
            { 
                await warmup.StartAsync(hostToken.Token); 
            } 
            catch 
            { 
                /* ignore */ 
            }

            // Assert
            await service.Received(1).GetAllAsync(Arg.Any<CancellationToken>()); // ensure it was called
            Assert.False(observed.CanBeCanceled); // we pass CancellationToken.None by design
        }

        [Fact]
        public async Task StartAsync_WhenServiceReturnsEmpty_ShouldContinue()
        {
            // Arrange
            var service = CreateService(Array.Empty<DisasterCard>());

            var warmup = CreateWarmupService(service);

            // Act
            await warmup.StartAsync(CancellationToken.None);

            // Assert
            await service.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StopAsync_WhenCalled_ShouldNotCallService()
        {
            // Arrange
            var service = CreateService(_cards);

            var warmup = CreateWarmupService(service);

            // Act
            await warmup.StopAsync(CancellationToken.None);

            // Assert
            await service.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task StopAsync_WhenCancelled_ShouldNotThrow()
        {
            // Arrange
            var service = CreateService(_cards);

            var warmup = CreateWarmupService(service);

            using var token = new CancellationTokenSource();
            await token.CancelAsync();

            // Act
            await warmup.StopAsync(token.Token);

            // Assert
            await service.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
        }

        private static IDisasterCardService CreateService(IReadOnlyList<DisasterCard> cards)
        {
            return CreateService(cards, 0);
        }

        private static IDisasterCardService CreateService(IReadOnlyList<DisasterCard> cards, int delay)
        {
            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync(Arg.Any<CancellationToken>())
                  .Returns(async _ =>
                  {
                      await Task.Delay(delay);
                      return cards;
                  });

            return service;
        }

        private static DisasterCardWarmupHostedService CreateWarmupService(IDisasterCardService service)
        {
            var options = new DisasterCardWarmupOptions
            {
                Enabled = true,
                Timeout = TimeSpan.FromSeconds(10)
            };

            return CreateWarmupService(options, service);
        }

        private static DisasterCardWarmupHostedService CreateWarmupService(DisasterCardWarmupOptions options, IDisasterCardService service)
        {
            var logger = Substitute.For<ILogger<DisasterCardWarmupHostedService>>();

            return new DisasterCardWarmupHostedService(Options.Create(options), service, logger);
        }
    }
}