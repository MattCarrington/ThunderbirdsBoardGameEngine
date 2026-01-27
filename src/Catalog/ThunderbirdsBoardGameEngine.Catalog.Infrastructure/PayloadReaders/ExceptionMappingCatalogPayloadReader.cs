using System.Security;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PayloadReaders
{
    internal class ExceptionMappingCatalogPayloadReader<TManifest> : ICatalogPayloadReader<TManifest> where TManifest : ICatalogManifest
    {
        private readonly ICatalogPayloadReader<TManifest> _inner;

        public ExceptionMappingCatalogPayloadReader(ICatalogPayloadReader<TManifest> inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<Payload<TManifest>> ReadAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                return await _inner.ReadAsync(filePath, cancellationToken);
            }
            catch (IOException ex) when (
                ex is FileNotFoundException ||
                ex is DirectoryNotFoundException ||
                ex is DriveNotFoundException)
            {
                throw CatalogDataAccessException.SourceNotFound(filePath, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw CatalogDataAccessException.AccessDenied(filePath, ex);
            }
            catch (SecurityException ex)
            {
                throw CatalogDataAccessException.AccessDenied(filePath, ex);
            }
            catch (IOException ex)
            {
                throw CatalogDataAccessException.SourceUnreadable(filePath, ex);
            }
            catch (ObjectDisposedException ex)
            {
                throw CatalogDataAccessException.SourceUnreadable(filePath, ex);
            }
            catch (JsonException ex)
            {
                throw CatalogDataAccessException.BadJson(filePath, ex);
            }
            catch (InvalidDataException ex)
            {
                throw CatalogDataAccessException.BadJson(filePath, ex);
            }
            catch (InvalidOperationException ex)
            {
                throw CatalogDataAccessException.BadJson(filePath, ex);
            }
            catch (NotSupportedException ex)
            {
                throw CatalogDataAccessException.BadJson(filePath, ex);
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
                throw CatalogDataAccessException.Unknown(filePath, ex);
            }
        }
    }
}
