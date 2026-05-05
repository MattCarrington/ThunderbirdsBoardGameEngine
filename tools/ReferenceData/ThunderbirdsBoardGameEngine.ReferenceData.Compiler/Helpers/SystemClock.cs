using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers
{
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
