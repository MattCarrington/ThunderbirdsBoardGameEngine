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
        private readonly IDisasterCardDeserializer _deserializer;
        private readonly IDisasterCardMapper _mapper;
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
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

                var dtos = _deserializer.Deserialize(payload.RawData);

                if (dtos is null || dtos.Count == 0)
                {
                    throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("Deserialized Disaster Card deck is null or empty"));
                }

                if (dtos.Any(dto => dto is null))
                {
                    throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("One or more Disaster Card DTOs are null"));
                }

                if (payload.Manifest.ItemCount != dtos.Count)
                {
                    throw CatalogDataAccessException.BadJson(_filePath,
                        new InvalidDataException($"Manifest itemCount {payload.Manifest.ItemCount} != deserialized count {dtos.Count}"));
                }

                return dtos.Select(_mapper.Map).ToList();
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
                ex is not StackOverflowException)   /// Added for clarity even though can't be caught anyway
            {
                throw CatalogDataAccessException.Unknown(_filePath, ex);
            }
        }

        private async Task<Stream> ValidateStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (stream is null)
            {
                throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("Opened stream is null"));
            }

            if (!stream.CanRead)
            {
                throw CatalogDataAccessException.SourceUnreadable(_filePath, new InvalidDataException("Opened stream is not readable"));
            }

            // Normalise non-seekable
            if (!stream.CanSeek)
            {
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms, 81920, cancellationToken).ConfigureAwait(false);
                await stream.DisposeAsync().ConfigureAwait(false);
                ms.Position = 0;
                stream = ms;
            }

            // Empty?
            if (stream.Length == 0 || stream.Position == stream.Length)
            {
                throw CatalogDataAccessException.DataMissing(_filePath,
                    new InvalidDataException("Opened stream is empty."));
            }

            // Peek for BOM + JSON whitespace only
            long position = stream.Position;
            var buf = new byte[Math.Min(4096, (int)(stream.Length - position))];
            int n = await stream.ReadAsync(buf.AsMemory(0, buf.Length), cancellationToken).ConfigureAwait(false);
            stream.Position = position;

            int i = 0;

            if (position == 0 && n >= 3 && buf[0] == Utf8Bom[0] && buf[1] == Utf8Bom[1] && buf[2] == Utf8Bom[2])
            {
                i = 3;
            }

            for (; i < n; i++)
            {
                byte b = buf[i];
                if (b is not (0x20 /*SP*/ or 0x09 /*HT*/ or 0x0A /*LF*/ or 0x0D /*CR*/))
                {
                    return stream; // found content
                }
            }

            if (position + n >= stream.Length)
            {
                throw CatalogDataAccessException.DataMissing(_filePath,
                    new InvalidDataException("Catalog data file contains no JSON content."));
            }

            // Scan the rest once
            var chunk = new byte[8192];
            long saved = stream.Position;
            stream.Position = position + n;
            int read;

            while ((read = await stream.ReadAsync(chunk.AsMemory(0, chunk.Length), cancellationToken).ConfigureAwait(false)) > 0)
            {
                for (int j = 0; j < read; j++)
                {
                    var b = chunk[j];
                    if (b is not (0x20 or 0x09 or 0x0A or 0x0D))
                    {
                        stream.Position = saved;
                        return stream;
                    }
                }
            }

            stream.Position = saved;

            throw CatalogDataAccessException.DataMissing(_filePath,
                new InvalidDataException("Catalog data file contains no JSON content."));
        }
    }
}
