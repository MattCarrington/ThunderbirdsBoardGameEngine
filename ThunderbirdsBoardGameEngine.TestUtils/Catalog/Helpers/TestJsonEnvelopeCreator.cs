using System.Text.Json;
using System.Text.Json.Nodes;
using ThunderbirdsBoardGameEngine.Catalog.Format.Hashing;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers
{
    public static class TestJsonEnvelopeCreator
    {
        private static readonly JsonSerializerOptions WriteOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static string EnvelopArrayFile(
            string arrayJsonAbsolutePath,
            string catalog = "DisasterCards")
        {
            var json = File.ReadAllText(arrayJsonAbsolutePath);
            return EnvelopArrayJsonToTempPath(json, catalog);
        }

        // Optional: in case you later want zero disk I/O with an in-memory provider
        public static string EnvelopArrayToString(
            string arrayJson,
            string catalog = "DisasterCards")
        {
            var (envelope, _) = BuildEnvelope(arrayJson, catalog);
            return envelope.ToJsonString(WriteOptions);
        }

        private static string EnvelopArrayJsonToTempPath(string arrayJson, string catalog)
        {
            var (envelope, _) = BuildEnvelope(arrayJson, catalog);
            var tempPath = Path.Combine(Path.GetTempPath(),
                $"disaster-cards-enveloped-{Path.GetRandomFileName()}.json");
            File.WriteAllText(tempPath, envelope.ToJsonString(WriteOptions));
            return tempPath;
        }

        private static (JsonObject envelope, string checksum) BuildEnvelope(
            string arrayJson,
            string catalog
            )
        {
            // Accept either a bare array or an already-enveloped object
            using var doc = JsonDocument.Parse(arrayJson);

            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("data", out _))
            {
                // Already enveloped: no changes
                var obj = JsonNode.Parse(arrayJson)!.AsObject();
                return (obj, "");
            }

            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException("Unsupported JSON shape: expected array or { meta, data }.");

            // Compute checksum using your canonical rules
            // (hash of the 'data' element, serialised with CanonicalJson.Options)
            var dataChecksum = CatalogChecksum.ComputeForDataElement(doc.RootElement);
            
            // Build envelope
            var dataNode = JsonNode.Parse(arrayJson)!.AsArray(); // re-use original structure as 'data'
            var meta = new JsonObject
            {
                ["catalog"] = catalog,
                ["schemaVersion"] = "1.0",
                ["contentVersion"] = "test",
                ["generatedAtUtc"] = "1970-01-01T00:00:00Z",
                ["itemCount"] = dataNode.Count,
                ["checksum"] = new JsonObject
                {
                    ["algorithm"] = CatalogChecksum.Algorithm, // "sha256"
                    ["value"] = dataChecksum
                }
            };

            var envelope = new JsonObject
            {
                ["meta"] = meta,
                ["data"] = dataNode
            };

            return (envelope, dataChecksum);
        }

        public static string CreateEmptyTempFile()
        {
            var path = Path.Combine(Path.GetTempPath(), $"empty-{Path.GetRandomFileName()}.json");
            using var _ = File.Create(path); // zero-byte
            return path;
        }
    }
}

