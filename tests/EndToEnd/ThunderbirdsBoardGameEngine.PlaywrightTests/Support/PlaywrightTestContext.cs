namespace ThunderbirdsBoardGameEngine.PlaywrightTests.Support
{
    public sealed class PlaywrightTestContext
    {
        public Guid? GameId { get; set; }

        public Guid RequireGameId()
        {
            return GameId ?? throw new InvalidOperationException("No game has been created for this scenario.");
        }
    }
}
