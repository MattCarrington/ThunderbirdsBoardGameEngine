using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Deserializers
{
    public static class JsonParser
    {

        public static JsonElement ParseElement(string json)
        {
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement.Clone(); // important: clone for lifetime
        }
    }
}