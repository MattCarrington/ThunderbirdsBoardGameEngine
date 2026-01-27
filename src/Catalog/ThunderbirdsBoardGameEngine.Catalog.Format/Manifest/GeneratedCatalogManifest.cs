namespace ThunderbirdsBoardGameEngine.Catalog.Format.Manifest
{
    public sealed record GeneratedCatalogManifest : SimpleCatalogManifest
    {
        public required DateTime GeneratedAtUtc { get; init; }

        public required int ItemCount { get; init; }

        public required Checksum Checksum { get; init; }

        public required ToolInfo ToolInfo { get; init; }

        public string? MinAppVersion { get; init; }
    }

}
