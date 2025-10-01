using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Serialization.Converters;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories
{
    internal class JsonDisasterCardRepository : IDisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;
        private readonly IFileReader _fileReader;

        public JsonDisasterCardRepository(IOptions<CardDataOptions> options, IFileReader fileReader)
        {
            // Options already validated at startup
            _filePath = options.Value.DisasterCardsFilePath;            
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));

            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new BonusConverter());
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"[DEBUG] Looking for file at: {Path.GetFullPath(_filePath)}");

            await using var stream = await _fileReader.OpenReadAsync(_filePath, cancellationToken);

            var cards = await JsonSerializer.DeserializeAsync<List<DisasterCard>>(stream, _options, cancellationToken);

            if (cards is null || cards.Count == 0)
            {
                return Array.Empty<DisasterCard>();
            }

            DisasterCardValidator.ValidateAll(cards);

            return cards;
        }
    }
}
