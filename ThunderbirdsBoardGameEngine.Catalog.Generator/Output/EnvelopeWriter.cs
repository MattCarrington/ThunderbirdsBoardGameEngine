using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Output
{
    public static class EnvelopeWriter
    {
        public static CatalogEnvelope<DisasterCardCatalogDto> BuildEnvelope(IReadOnlyList<DisasterCardCatalogDto> items, JsonSerializerOptions options)
        {
            if (items is null || items.Count == 0)
            {
                throw new InvalidDataException("No items");
            }

            if (items.Select(c => c.Id).Distinct().Count() != items.Count)
            {
                throw new InvalidDataException("Duplicate item IDs detected");
            }

            var checksum = CatalogChecksum.ComputeChecksum(items, options);

            var meta = new CatalogManifest
            {
                Checksum = new Checksum
                {
                    Algorithm = CatalogChecksum.Algorithm,
                    Value = checksum
                },
                Catalog = "DisasterCards",
                ItemCount = items.Count,
                SchemaVersion = "1.0",
                GeneratedAtUtc = DateTime.UtcNow,
                ContentVersion = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Annotations = new()
                {
                    ["idStrategy"] = "orderBy slug(name)|rescueType|location → 0..N-1",
                    ["codeStrategy"] = "slug(name), lowercase ASCII '-'",
                    ["serializer"] = ".NET8 STJ camelCase + ignoreNull; checksum over data only"
                },
                ToolInfo = new ToolInfo 
                { 
                    Name = "Catalog.Generator", 
                    Version = "1.0.0" 
                },
                MinAppVersion = null
            };

            return new CatalogEnvelope<DisasterCardCatalogDto>
            {
                Meta = meta,
                Data = items
            };
        }
    }
}
