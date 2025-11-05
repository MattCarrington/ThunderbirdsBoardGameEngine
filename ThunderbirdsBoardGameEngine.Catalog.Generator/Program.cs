using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Importers;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Output;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("📥 Importing Disaster Card Data...");

            var importer = new DisasterCardImporter();
            var cards = importer.ImportDisasterCardData("Resources/DisasterCards.xlsx");

            var options = CatalogJson.Catalog;
            JsonWriter.WriteJson(cards, "Output/DisasterCards.json", options);

            Console.WriteLine("✅ DisasterCards.json written.");
        }
    }
}
