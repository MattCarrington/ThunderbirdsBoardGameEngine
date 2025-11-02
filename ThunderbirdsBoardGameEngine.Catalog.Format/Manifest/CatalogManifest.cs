namespace ThunderbirdsBoardGameEngine.Catalog.Format.Manifest
{
    public sealed record CatalogManifest
    {
        public required string Catalog { get; init; }          // "DisasterCards"

        public required string SchemaVersion { get; init; }    // "1.0"

        public required string ContentVersion { get; init; }   // e.g., "2025-11-02"

        public required DateTime GeneratedAtUtc { get; init; }

        public required int ItemCount { get; init; }

        public required Checksum Checksum { get; init; }

        public Dictionary<string, string>? Annotations { get; init; }

        public ToolInfo? ToolInfo { get; init; }

        public string? MinAppVersion { get; init; }
    }
}
