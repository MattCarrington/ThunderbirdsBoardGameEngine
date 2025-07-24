using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using ThunderbirdsBoardGameEngine.GameData.Api.Converters;
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Repositories
{
    public class DisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;

        public DisasterCardRepository(string filepath)
        {
            _filePath = filepath;

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
