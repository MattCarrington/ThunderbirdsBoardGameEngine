using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs
{
    public class JsonOptionsSnapshot : IOptionsSnapshot<JsonSerializerOptions>
    {
        private readonly JsonSerializerOptions _options;

        public JsonOptionsSnapshot(JsonSerializerOptions options)
        {
            _options = options;
        }

        public JsonSerializerOptions Value
        {
            get
            {
                return _options;
            }
        }

        public JsonSerializerOptions Get(string name)
        {
            return _options;
        }
    }    
}
