using System.Security.Cryptography;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Hashing
{
    public static class CatalogChecksum
    {
        public const string Algorithm = "sha256";

        public static string ComputeChecksum(IReadOnlyList<DisasterCardCatalogDto> items)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(items, CanonicalJson.Options);
            
            Span<byte> hash = stackalloc byte[32];
            SHA256.HashData(json, hash);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static string ComputeForDataElement(JsonElement dataElement)
        {
            var utf8 = JsonSerializer.SerializeToUtf8Bytes(dataElement, CanonicalJson.Options);
            Span<byte> hash = stackalloc byte[32];
            SHA256.HashData(utf8, hash);
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        public static bool Verify(IReadOnlyList<DisasterCardCatalogDto> data, string expectedLowerHex)
        {
            return string.Equals(ComputeChecksum(data), expectedLowerHex, StringComparison.OrdinalIgnoreCase);
        }
    }
}
