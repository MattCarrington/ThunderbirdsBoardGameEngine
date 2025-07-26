using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using ThunderbirdsBoardGameEngine.GameData.Api.Converters;
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Repositories
{
    public class DisasterCardRepository : IDisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        public DisasterCardRepository(IOptions<CardDataOptions> options)
        {
            _filePath = options.Value.DisasterCardFilePath;

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new BonusConverter());
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync()
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException($"Disaster card file not found: {_filePath}");

            await using var stream = File.OpenRead(_filePath);

            var cards = await JsonSerializer.DeserializeAsync<List<DisasterCard>>(stream, _options, CancellationToken.None);

            return cards ?? [];
        }
    }
}
