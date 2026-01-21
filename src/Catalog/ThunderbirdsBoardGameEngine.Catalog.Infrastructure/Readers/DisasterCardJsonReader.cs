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
    internal sealed class DisasterCardJsonReader : IDisasterCardReader
    {
        private readonly string _filePath;
        private readonly ICatalogPayloadReader<GeneratedCatalogManifest> _catalogPayloadReader;
        private readonly IDisasterCardDeserializer _disasterCardDeserializer;
        private readonly IDisasterCardMapper _disasterCardMapper;
        private readonly ILogger<DisasterCardJsonReader> _logger;

        public DisasterCardJsonReader(
            IOptions<DisasterCardJsonOptions> options,
            ICatalogPayloadReader<GeneratedCatalogManifest> catalogPayloadReader,
            IDisasterCardDeserializer deserializer,
            IDisasterCardMapper mapper,
            ILogger<DisasterCardJsonReader> logger)
        {
            // Options already validated at startup
            if (options is null || string.IsNullOrWhiteSpace(options.Value.FilePath))
            {
                throw new ArgumentException("DisasterCardJsonOptions or its FilePath is invalid", nameof(options));
            }

            _filePath = options.Value.FilePath;
            _catalogPayloadReader = catalogPayloadReader ?? throw new ArgumentNullException(nameof(catalogPayloadReader));
            _disasterCardDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _disasterCardMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Loading Disaster Cards from {Path}", _filePath);

            var payload = await _catalogPayloadReader.ReadAsync(_filePath, cancellationToken);

            IReadOnlyList<DisasterCardCatalogDto> dtos;

            try
            {
                dtos = _disasterCardDeserializer.Deserialize(payload.RawData);
            }
            catch (JsonException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }
            catch (NotSupportedException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }

            ValidateDisasterCardCatalogDtos(dtos, payload.Manifest.ItemCount);

            var cards = dtos.Select(_disasterCardMapper.Map).ToList();

            _logger.LogInformation(
                "Successfully loaded Disaster Card catalog. CardCount = {CardCount}, ManifestItemCount = {ManifestItemCount}",
                cards.Count,
                payload.Manifest.ItemCount);

            return cards;
        }

        private void ValidateDisasterCardCatalogDtos(IReadOnlyList<DisasterCardCatalogDto>? dtos, int itemCount)
        {
            if (dtos is null || dtos.Count == 0)
            {
                throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("Deserialized Disaster Card deck is null or empty"));
            }

            if (dtos.Any(dto => dto is null))
            {
                throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("One or more Disaster Card DTOs are null"));
            }

            if (itemCount != dtos.Count)
            {
                throw CatalogDataAccessException.BadJson(_filePath,
                    new InvalidDataException($"Manifest itemCount {itemCount} != deserialized count {dtos.Count}"));
            }
        }
    }
}
