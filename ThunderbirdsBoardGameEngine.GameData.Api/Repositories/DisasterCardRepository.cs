using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.Serialization.Converters;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Repositories
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

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync()
        {
            Console.WriteLine($"[DEBUG] Looking for file at: {Path.GetFullPath(_filePath)}");

            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Disaster card file not found: {_filePath}");

            await using var stream = File.OpenRead(_filePath);

            var cards = await JsonSerializer.DeserializeAsync<List<DisasterCard>>(stream, _options, CancellationToken.None);

            return cards ?? [];
        }
    }
}
