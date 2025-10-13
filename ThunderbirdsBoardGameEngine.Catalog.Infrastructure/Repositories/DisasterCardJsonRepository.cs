using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories
{
    internal class DisasterCardJsonRepository : IDisasterCardRepository
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly IFileReader _fileReader;
        private readonly ILogger<DisasterCardJsonRepository> _logger;

        public DisasterCardJsonRepository(
            IOptions<DisasterCardJsonOptions> options, IFileReader fileReader, ILogger<DisasterCardJsonRepository> logger, IOptionsSnapshot<JsonSerializerOptions> jsonOptions)
        {
            // Options already validated at startup
            _filePath = options.Value.FilePath;
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            _logger = logger;
            _jsonOptions = jsonOptions.Get(CatalogJson.Name) ?? throw new ArgumentNullException(nameof(jsonOptions));
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Loading Disaster Cards from {Path}", _filePath);

            try
            {
                List<DisasterCard>? cards;

                await using var stream = await _fileReader.OpenReadAsync(_filePath, cancellationToken);
                                
                try
                {
                    cards = await JsonSerializer.DeserializeAsync<List<DisasterCard>>(stream, _jsonOptions, cancellationToken);
                }
                catch (JsonException ex)
                {
                    throw CatalogDataAccessException.BadJson(_filePath, ex);
                }
                catch (NotSupportedException ex)
                {
                    throw CatalogDataAccessException.BadJson(_filePath, ex);
                }

                if (cards is null || cards.Count == 0)
                {
                    throw CatalogDataAccessException.DataMissing(_filePath, new InvalidDataException("Thunderbirds Disaster Card deck is null or empty"));
                }

                _logger.LogDebug("Loaded {Count} disaster cards from {Path}", cards.Count, _filePath);

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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (DisasterCardValidationException)
            {
                throw;
            }
            catch (CatalogDataAccessException)
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
    }
}
