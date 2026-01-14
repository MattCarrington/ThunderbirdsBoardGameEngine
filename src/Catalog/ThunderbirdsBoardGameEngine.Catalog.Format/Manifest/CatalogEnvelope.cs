namespace ThunderbirdsBoardGameEngine.Catalog.Format.Manifest
{
    public sealed record CatalogEnvelope<TItem> where TItem : class
    {
        public required GeneratedCatalogManifest Meta { get; init; }

        public required IReadOnlyList<TItem> Data { get; init; }
    }
}
