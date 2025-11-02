namespace ThunderbirdsBoardGameEngine.Catalog.Format.Manifest
{
    public sealed record ToolInfo 
    { 
        public required string Name { get; init; } 
        
        public string? Version { get; init; } 
        
        public string? GitCommit { get; init; } 
    }
}
