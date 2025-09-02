using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Serialization.Converters;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories
{
    internal class JsonDisasterCardRepository : IDisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        public JsonDisasterCardRepository(IOptions<CardDataOptions> options)
        {
            var filePath = options.Value.DisasterCardsFilePath;
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("DisasterCardsFilePath must be set in configuration");

            _filePath = filePath;

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new BonusConverter());
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync()
        {
            Console.WriteLine($"[DEBUG] Looking for file at: {Path.GetFullPath(_filePath)}");

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Disaster card file not found: {_filePath}");

            await using var stream = File.OpenRead(_filePath);

            var cards = await JsonSerializer.DeserializeAsync<List<DisasterCard>>(stream, _options, CancellationToken.None);

            if (cards is null)
            {
                Console.WriteLine("[DEBUG] No disaster cards found in the file.");
                return [];
            }

            DisasterCardValidator.ValidateAll(cards);

            return cards;
        }
    }
}
