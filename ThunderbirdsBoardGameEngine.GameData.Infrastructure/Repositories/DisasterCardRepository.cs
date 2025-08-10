using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Domain.Validators;
using ThunderbirdsBoardGameEngine.Serialization.Converters;
using ThunderbirdsBoardGameEngine.GameData.Application.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.GameData.Infrastructure.Repositories
{
    public class DisasterCardRepository : IDisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        public DisasterCardRepository(IOptions<CardDataOptions> options)
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

        public async Task<DisasterCard> GetByIdAsync(int id)
        {
            var cards = await GetAllAsync();
            return cards.FirstOrDefault(card => card.Id == id);
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
