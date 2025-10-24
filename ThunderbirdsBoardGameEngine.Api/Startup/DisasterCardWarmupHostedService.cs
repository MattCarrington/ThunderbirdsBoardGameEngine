using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;

namespace ThunderbirdsBoardGameEngine.Api.Startup
{
    public sealed class DisasterCardWarmupHostedService : IHostedService
    {
        private readonly IOptions<DisasterCardWarmupOptions> _options;
        private readonly IDisasterCardService _disasterCardService;
        private readonly ILogger<DisasterCardWarmupHostedService> _logger;

        public DisasterCardWarmupHostedService(IOptions<DisasterCardWarmupOptions> options, IDisasterCardService disasterCardService, ILogger<DisasterCardWarmupHostedService> logger)
        {
            _options = options;
            _disasterCardService = disasterCardService ?? throw new ArgumentNullException(nameof(disasterCardService));
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_options.Value.Enabled)
            {
                _logger.LogInformation("Disaster Card warmup is disabled. Skipping warmup during startup.");
                return;
            }

            using var token = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            token.CancelAfter(_options.Value.Timeout);

            var cards = await _disasterCardService.GetAllAsync(CancellationToken.None).WaitAsync(token.Token);
            _logger.LogInformation("Successfully loaded {Count} Disaster Cards during startup.", cards.Count);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}


