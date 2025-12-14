using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers
{
    internal sealed class DisasterCardJsonReader : IDisasterCardReader
    {
        private static readonly byte[] Utf8Bom = { 0xEF, 0xBB, 0xBF };

        private readonly string _filePath;
        private readonly IFileOpener _fileOpener;
        private readonly IEnvelopeParser _envelopeParser;
        private readonly IDisasterCardDeserializer _disasterCardDeserializer;
        private readonly IDisasterCardMapper _disasterCardMapper;
        private readonly ILogger<DisasterCardJsonReader> _logger;

        public DisasterCardJsonReader(
            IOptions<DisasterCardJsonOptions> options,
            IFileOpener fileOpener,
            IEnvelopeParser envelopeParser,
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
            _envelopeParser = envelopeParser ?? throw new ArgumentNullException(nameof(envelopeParser));
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

                using var validatedStream = await ValidateStreamAsync(stream, cancellationToken);

                var payload = await _envelopeParser.ReadEnvelopeAsync(validatedStream, cancellationToken);

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

        private void ValidateDisasterCardCatalogDtos(IReadOnlyList<Format.Dtos.DisasterCardCatalogDto>? dtos, int itemCount)
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

        private async Task<Stream> ValidateStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validatedStream = await EnsureReadableSeekableStreamAsync(stream, cancellationToken);

            // Empty?
            if (validatedStream.Length == 0 || validatedStream.Position == validatedStream.Length)
            {
                throw CatalogDataAccessException.DataMissing(_filePath,
                    new InvalidDataException("Opened stream is empty."));
            }

            if (!await HasNonWhitespaceJsonContentAsync(validatedStream, cancellationToken)
                 .ConfigureAwait(false))
            {
                throw CatalogDataAccessException.DataMissing(
                    _filePath,
                    new InvalidDataException("Catalog data file contains no JSON content."));
            }

            return validatedStream;
        }

        private async Task<Stream> EnsureReadableSeekableStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (stream is null)
            {
                throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("Opened stream is null"));
            }

            if (!stream.CanRead)
            {
                throw CatalogDataAccessException.SourceUnreadable(_filePath, new InvalidDataException("Opened stream is not readable"));
            }

            // Normalise non-seekable
            if (stream.CanSeek)
            {
                return stream;
            }

            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, 81920, cancellationToken).ConfigureAwait(false);
            await stream.DisposeAsync().ConfigureAwait(false);
            memoryStream.Position = 0;

            return memoryStream;
        }

        private static async Task<bool> HasNonWhitespaceJsonContentAsync(Stream stream, CancellationToken cancellationToken)
        {
            var position = stream.Position;
            var buffer = new byte[Math.Min(4096, (int)(stream.Length - position))];
            var read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
            stream.Position = position;

            if (read == 0)
            {
                return false;
            }

            int index = SkipUtf8BomIfPresent(buffer, read, position);

            if (ContainsNonWhitespace(buffer, index, read))
            {
                return true;
            }

            if (position + read >= stream.Length)
            {
                return false;
            }

            // Scan the rest
            var chunk = new byte[8192];
            var saved = stream.Position;
            stream.Position = position + read;

            int chunkRead;

            while ((chunkRead = await stream.ReadAsync(chunk.AsMemory(0, chunk.Length), cancellationToken).ConfigureAwait(false)) > 0)
            {
                if (ContainsNonWhitespace(chunk, 0, chunkRead))
                {
                    stream.Position = saved;
                    return true;
                }
            }

            stream.Position = saved;

            return false;
        }

        private static int SkipUtf8BomIfPresent(byte[] buffer, int count, long position)
        {
            if (position == 0 &&
                count >= 3 &&
                buffer[0] == Utf8Bom[0] &&
                buffer[1] == Utf8Bom[1] &&
                buffer[2] == Utf8Bom[2])
            {
                return 3;
            }

            return 0;
        }

        private static bool ContainsNonWhitespace(byte[] buffer, int start, int count)
        {
            for (int i = start; i < count; i++)
            {
                var b = buffer[i];

                if (b is not (0x20 or 0x09 or 0x0A or 0x0D))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
