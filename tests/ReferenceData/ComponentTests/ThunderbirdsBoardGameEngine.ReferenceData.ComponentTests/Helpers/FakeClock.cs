using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers
{
    public sealed class FakeClock : IClock
    {
        public DateTimeOffset UtcNow { get; set; }

        public FakeClock(DateTimeOffset utcNow)
        {
            UtcNow = utcNow;
        }
    }
}
