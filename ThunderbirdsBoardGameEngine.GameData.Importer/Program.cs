using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Importer.Importers;
using ThunderbirdsBoardGameEngine.GameData.Importer.Output;

namespace ThunderbirdsBoardGameEngine.GameData.Importer
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
