using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs
{
    public class JsonOptionsMonitor : IOptionsMonitor<JsonSerializerOptions>
    {
        private readonly JsonSerializerOptions _options;

        public JsonOptionsMonitor(JsonSerializerOptions options)
        {
            _options = options;
        }

        public JsonSerializerOptions CurrentValue => _options;

        public JsonSerializerOptions Get(string? name)
        {
            return _options;
        }

        public IDisposable? OnChange(Action<JsonSerializerOptions, string?> listener)
        {
            return NullDisposable.Instance;
        }

        private sealed class NullDisposable : IDisposable
        {
            public static readonly NullDisposable Instance = new();

            public void Dispose() { }
        }
    }
}
