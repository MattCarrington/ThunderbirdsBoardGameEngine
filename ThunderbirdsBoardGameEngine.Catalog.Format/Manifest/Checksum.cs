namespace ThunderbirdsBoardGameEngine.Catalog.Format.Manifest
{
    public sealed record Checksum
    {
        public required string Algorithm { get; init; }

        public required string Value { get; init; }
    }
}
