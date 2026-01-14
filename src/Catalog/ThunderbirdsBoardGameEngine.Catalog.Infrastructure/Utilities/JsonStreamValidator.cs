using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal class JsonStreamValidator : IJsonStreamValidator
    {
        private static readonly byte[] Utf8Bom = { 0xEF, 0xBB, 0xBF };

        public async Task<Stream> ValidateStreamAsync(Stream stream, string filePath, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var validatedStream = await EnsureReadableSeekableStreamAsync(stream, filePath, cancellationToken);

            // Empty?
            if (validatedStream.Length == 0 || validatedStream.Position == validatedStream.Length)
            {
                throw CatalogDataAccessException.DataMissing(filePath,
                    new InvalidDataException("Opened stream is empty."));
            }

            if (!await HasNonWhitespaceJsonContentAsync(validatedStream, cancellationToken)
                 .ConfigureAwait(false))
            {
                throw CatalogDataAccessException.DataMissing(
                    filePath,
                    new InvalidDataException("Catalog data file contains no JSON content."));
            }

            return validatedStream;
        }

        private static async Task<Stream> EnsureReadableSeekableStreamAsync(Stream stream, string filePath, CancellationToken cancellationToken)
        {
            if (stream is null)
            {
                throw CatalogDataAccessException.DataMissing(filePath, new InvalidDataException("Opened stream is null"));
            }

            if (!stream.CanRead)
            {
                throw CatalogDataAccessException.SourceUnreadable(filePath, new InvalidDataException("Opened stream is not readable"));
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
