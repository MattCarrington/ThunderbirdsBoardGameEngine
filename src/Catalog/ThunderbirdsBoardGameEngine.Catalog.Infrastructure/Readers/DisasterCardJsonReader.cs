using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers
{
    internal sealed class DisasterCardJsonReader : IDisasterCardReader
    {
        private readonly string _filePath;
        private readonly IFileOpener _fileOpener;
        private readonly IJsonStreamValidator _jsonStreamValidator;
        private readonly IEnvelopeParser _envelopeParser;
        private readonly IGeneratedContentValidator _generatedContentValidator;
        private readonly IDisasterCardDeserializer _disasterCardDeserializer;
        private readonly IDisasterCardMapper _disasterCardMapper;
        private readonly ILogger<DisasterCardJsonReader> _logger;

        public DisasterCardJsonReader(
            IOptions<DisasterCardJsonOptions> options,
            IFileOpener fileOpener,
            IJsonStreamValidator jsonStreamValidator,
            IEnvelopeParser envelopeParser,
            IGeneratedContentValidator generatedContentValidator,
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
            _fileOpener = fileOpener ?? throw new ArgumentNullException(nameof(fileOpener));
            _jsonStreamValidator = jsonStreamValidator ?? throw new ArgumentNullException(nameof(jsonStreamValidator));
            _envelopeParser = envelopeParser ?? throw new ArgumentNullException(nameof(envelopeParser));
            _generatedContentValidator = generatedContentValidator ?? throw new ArgumentNullException(nameof(generatedContentValidator));
            _disasterCardDeserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _disasterCardMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug("Loading Disaster Cards from {Path}", _filePath);

            try
            {
                await using var stream = await _fileOpener.OpenReadAsync(_filePath, cancellationToken);

                using var validatedStream = await _jsonStreamValidator.ValidateStreamAsync(stream, _filePath, cancellationToken);

                var payload = await _envelopeParser.ReadEnvelopeAsync<GeneratedCatalogManifest>(validatedStream, cancellationToken);

                _generatedContentValidator.Validate<GeneratedCatalogManifest>(payload);

                var dtos = _disasterCardDeserializer.Deserialize(payload.RawData);

                ValidateDisasterCardCatalogDtos(dtos, payload.Manifest.ItemCount);

                var cards = dtos.Select(_disasterCardMapper.Map).ToList();

                _logger.LogInformation(
                    "Successfully loaded Disaster Card catalog. CardCount = {CardCount}, ManifestItemCount = {ManifestItemCount}",
                    cards.Count,
                    payload.Manifest.ItemCount);

                return cards;
            }
            catch (IOException ex) when (
                ex is FileNotFoundException ||
                ex is DirectoryNotFoundException ||
                ex is DriveNotFoundException)
            {
                throw CatalogDataAccessException.SourceNotFound(_filePath, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw CatalogDataAccessException.AccessDenied(_filePath, ex);
            }
            catch (SecurityException ex)
            {
                throw CatalogDataAccessException.AccessDenied(_filePath, ex);
            }
            catch (IOException ex)
            {
                throw CatalogDataAccessException.SourceUnreadable(_filePath, ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw CatalogDataAccessException.SourceUnreadable(_filePath, ex);
            }
            catch (JsonException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }
            catch (InvalidDataException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }
            catch (NotSupportedException ex)
            {
                throw CatalogDataAccessException.BadJson(_filePath, ex);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (CatalogDataAccessException)
            {
                throw;
            }
            catch (DisasterCardValidationException)
            {
                throw;
            }
            catch (Exception ex) when (
                ex is not OutOfMemoryException &&
                ex is not AccessViolationException &&
                ex is not StackOverflowException)   // Added for clarity even though can't be caught anyway
            {
                throw CatalogDataAccessException.Unknown(_filePath, ex);
            }
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
