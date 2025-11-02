using System.Security.Cryptography;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Hashing
{
    public static class CatalogChecksum
    {
        public const string Algorithm = "sha256";

        public static string ComputeChecksum(IReadOnlyList<DisasterCardCatalogDto> items, JsonSerializerOptions options)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(items, options);
            
            Span<byte> hash = stackalloc byte[32];
            SHA256.HashData(json, hash);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static bool Verify(IReadOnlyList<DisasterCardCatalogDto> data, string expectedLowerHex, JsonSerializerOptions options)
        {
            return string.Equals(ComputeChecksum(data, options), expectedLowerHex, StringComparison.OrdinalIgnoreCase);
        }
    }
}
