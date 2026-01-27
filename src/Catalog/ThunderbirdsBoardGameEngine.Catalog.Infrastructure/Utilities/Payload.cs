using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal sealed record Payload<TManifest> where TManifest : ICatalogManifest
    {
        public required TManifest Manifest { get; init; }

        public required JsonElement RawData { get; init; }
    }
}
