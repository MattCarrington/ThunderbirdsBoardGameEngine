namespace ThunderbirdsBoardGameEngine.Api
{
    public sealed class PublicApiRateLimitOptions
    {
        public int PermitLimit { get; set; } = 100;
        public int WindowSeconds { get; set; } = 60;
        public int QueueLimit { get; set; } = 0;
    }
}
