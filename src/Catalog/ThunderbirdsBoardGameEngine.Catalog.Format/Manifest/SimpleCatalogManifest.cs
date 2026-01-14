namespace ThunderbirdsBoardGameEngine.Catalog.Format.Manifest
{
    public record SimpleCatalogManifest : ICatalogManifest
    {
        public required string Catalog { get; init; }

        public required string SchemaVersion { get; init; }

        public string? ContentVersion { get; init; }

        public Dictionary<string, string>? Annotations { get; init; }
    }

}
