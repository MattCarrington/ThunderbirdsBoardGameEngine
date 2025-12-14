using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal record struct Payload(CatalogManifest Manifest, JsonElement RawData);
}
