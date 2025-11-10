using System.Globalization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Importers;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Output;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator
{
    public static class Program
    {
        static int Main(string[] args)
        {
            try
            {
                CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-GB");

                var opts = ArgOptions.Parse(args);
                if (opts.ShowHelp)
                {
                    ArgOptions.PrintHelp();
                    return 0;
                }

                Console.WriteLine("📥 Importing Disaster Card Data...");

                var inputPath = opts.InputPath ?? "Resources/DisasterCards.xlsx";
                var outputPath = opts.OutputPath ?? "Output/DisasterCards.json";

                if (!File.Exists(inputPath))
                {
                    throw new FileNotFoundException($"Input Excel not found: {inputPath}");
                }

                var importer = new DisasterCardImporter();
                var cards = importer.ImportDisasterCardData(inputPath);

                var jsonOptions = opts.Pretty ? CanonicalJson.Pretty() : CatalogJson.Catalog;
                JsonWriter.WriteJson(cards, outputPath, jsonOptions);

                Console.WriteLine("✅ DisasterCards.json written.");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ An error occurred during processing.");
                Console.WriteLine(ex.Message);
                return 1;
            }
        }



        private sealed record ArgOptions(
            string InputPath,
            string OutputPath,
            bool Pretty,
            bool Validate,
            bool ShowHelp)
        {
            public static ArgOptions Parse(string[] args)
            {
                string? input = null, output = null;
                bool pretty = false, validate = false, help = false;

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--input":
                        case "-i":
                            input = args[++i];
                            break;
                        case "--out":
                        case "-o":
                            output = args[++i];
                            break;
                        case "--pretty":
                            pretty = true;
                            break;
                        case "--validate":
                            validate = true;
                            break;
                        case "--help":
                        case "-h":
                        case "/?":
                            help = true;
                            break;
                        default:
                            // tolerate `foo=bar` style
                            if (args[i].StartsWith("--input=")) input = args[i]["--input=".Length..];
                            else if (args[i].StartsWith("--out=")) output = args[i]["--out=".Length..];
                            else help = true;
                            break;
                    }
                }

                if (help || string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(output))
                {
                    return new ArgOptions(input ?? "", output ?? "", pretty, validate, ShowHelp: true);
                }

                return new ArgOptions(input!, output!, pretty, validate, ShowHelp: false);
            }

            public static void PrintHelp()
            {
                Console.WriteLine(@"Thunderbirds Catalogue Generator
                Usage:
                  dotnet run -c Release --project tools/Catalog.Generator -- --input <xlsx> --out <json> [--pretty] [--validate]

                Options:
                  -i, --input      Path to the Excel file (e.g., data/DisasterCards.xlsx)
                  -o, --out        Path to write JSON (e.g., catalog/DisasterCards.json)
                      --pretty     Indented JSON
                      --validate   Round-trip/enum validation after writing
                  -h, --help       Show this help
                ");
            }
        }
    }
}