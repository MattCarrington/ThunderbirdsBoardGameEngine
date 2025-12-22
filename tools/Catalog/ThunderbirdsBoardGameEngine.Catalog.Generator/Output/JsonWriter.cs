using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Output
{
    public static class JsonWriter
    {
        public static void WriteJson(IEnumerable<DisasterCardCatalogDto> cards, string outputPath, JsonSerializerOptions options)
        {
            var envelope = EnvelopeWriter.BuildEnvelope(cards.ToList(), options);
            var json = JsonSerializer.Serialize(envelope, CanonicalJson.Pretty());

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
