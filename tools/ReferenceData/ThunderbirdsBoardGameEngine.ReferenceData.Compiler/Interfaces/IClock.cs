namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }
}
