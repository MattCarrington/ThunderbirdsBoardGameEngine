namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests
{
    public sealed class EnvironmentVariableScope : IDisposable
    {
        private readonly string _name;
        private readonly string? _prev;

        public EnvironmentVariableScope(string name, string? value)
        {
            _name = name;
            _prev = Environment.GetEnvironmentVariable(name);
            Environment.SetEnvironmentVariable(name, value);
        }

        public void Dispose()
        {
            Environment.SetEnvironmentVariable(_name, _prev);
        }
    }
}
