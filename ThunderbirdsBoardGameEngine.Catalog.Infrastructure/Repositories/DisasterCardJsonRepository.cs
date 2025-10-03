using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Serialization.Converters;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories
{
    internal class DisasterCardJsonRepository : IDisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;
        private readonly IFileReader _fileReader;
        private readonly ILogger<DisasterCardJsonRepository> _logger;

        public DisasterCardJsonRepository(IOptions<DisasterCardJsonOptions> options, IFileReader fileReader, ILogger<DisasterCardJsonRepository> logger)
        {
            // Options already validated at startup
            _filePath = options.Value.FilePath;            
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            _logger = logger;
            
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new BonusConverter());
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Loading Disaster Cards from {Path}", _filePath);

            await using var stream = await _fileReader.OpenReadAsync(_filePath, cancellationToken);

            var cards = await JsonSerializer.DeserializeAsync<List<DisasterCard>>(stream, _options, cancellationToken);

            if (cards is null || cards.Count == 0)
            {
                _logger.LogWarning("No Disaster Cards found in the file {Path}. Returning empty list.", _filePath);
                return Array.Empty<DisasterCard>();
            }

            _logger.LogDebug("Deserialized {Count} disaster cards. Validating…", cards.Count);
            DisasterCardValidator.ValidateAll(cards);
            _logger.LogDebug("Loaded {Count} disaster cards from {Path}", cards.Count, _filePath);

            return cards;
        }
    }
}
