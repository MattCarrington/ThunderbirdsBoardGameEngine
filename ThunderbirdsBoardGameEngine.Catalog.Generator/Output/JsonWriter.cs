using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Output
{
    public static class JsonWriter
    {
        public static void WriteJson(IEnumerable<DisasterCardCatalogDto> cards, string outputPath, JsonSerializerOptions options)
        {
            var envelope = EnvelopeWriter.BuildEnvelope(cards.ToList(), options);
            var json = JsonSerializer.Serialize(envelope, options);

            // Resolve to an absolute path (treat relative as based on the current working dir)
            var fullPath = Path.GetFullPath(outputPath);

            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            { 
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(fullPath, json);
            Console.WriteLine($"✅ Wrote {cards.Count()} items to: {fullPath}");
        }

    }
}
