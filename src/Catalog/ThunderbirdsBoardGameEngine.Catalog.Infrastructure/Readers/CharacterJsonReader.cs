using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers
{
    internal sealed class CharacterJsonReader : ICharacterReader
    {
        private readonly string _filePath;
        private readonly ICatalogPayloadReader<SimpleCatalogManifest> _catlogPayloadReader;
        private readonly ICharacterDeserializer _characterDeserializer;
        private readonly ICharacterMapper _characterMapper;
        private readonly ILogger<CharacterJsonReader> _logger;

        public CharacterJsonReader(
            IOptions<CharacterJsonOptions> options,
            ICatalogPayloadReader<SimpleCatalogManifest> catalogPayloadReader,
            ICharacterDeserializer characterDeserializer,
            ICharacterMapper characterMapper,
            ILogger<CharacterJsonReader> logger)
        {
            if (options is null || string.IsNullOrWhiteSpace(options.Value.FilePath))
            {
                throw new ArgumentException("CharacterJsonOptions or its FilePath is invalid", nameof(options));
            }

            _filePath = options.Value.FilePath;
            _catlogPayloadReader = catalogPayloadReader ?? throw new ArgumentNullException(nameof(catalogPayloadReader));
            _characterDeserializer = characterDeserializer ?? throw new ArgumentNullException(nameof(characterDeserializer));
            _characterMapper = characterMapper ?? throw new ArgumentNullException(nameof(characterMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<CharacterDefinition>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Loading Characters from {Path}", _filePath);

            var payload = await _catlogPayloadReader.ReadAsync(_filePath, cancellationToken);

            IReadOnlyList<CharacterCatalogDto> dtos;

            try
            {
                dtos = _characterDeserializer.Deserialize(payload.RawData);
            }
            catch (JsonException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }
            catch (NotSupportedException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }

            ValidateCharacterCatalogDtos(dtos);

            var characters = dtos.Select(_characterMapper.Map).ToList();

            _logger.LogDebug("Loaded {Count} Characters from {Path}", characters.Count, _filePath);

            return characters;
        }

        private void ValidateCharacterCatalogDtos(IReadOnlyList<CharacterCatalogDto>? dtos)
        {
            if (dtos is null || dtos.Count == 0)
            {
                throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("Deserialized Characters is null or empty"));
            }

            if (dtos.Any(dto => dto is null))
            {
                throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("One or more Character DTOs are null"));
            }
        }
    }
}
