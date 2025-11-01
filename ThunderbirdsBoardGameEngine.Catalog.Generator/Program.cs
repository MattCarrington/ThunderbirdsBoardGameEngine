using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Importers;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Output;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("📥 Importing Disaster Cards...");

            var importer = new DisasterCardImporter();
            List<DisasterCard> cards = importer.Import("Resources/DisasterCards.xlsx");

            var options = JsonWriter.CreateDefaultOptions();
            JsonWriter.WriteJson(cards, "Output/DisasterCards.json", options);

            Console.WriteLine("✅ DisasterCards.json written.");
        }
    }
}
