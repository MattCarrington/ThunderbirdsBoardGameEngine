namespace ThunderbirdsBoardGameEngine.Catalog.Application.Configuration
{
    public sealed class DisasterCardWarmupOptions
    {
        public bool Enabled { get; set; }

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
