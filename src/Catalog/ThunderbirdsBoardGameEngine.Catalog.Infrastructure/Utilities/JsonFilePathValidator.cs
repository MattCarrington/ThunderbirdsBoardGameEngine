using System.IO.Abstractions;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities
{
    internal static class JsonFilePathValidator
    {
        public static IReadOnlyList<string> ValidateJsonFilePath(string configKey, string? path, IFileSystem fileSystem)
        {
            var failures = new List<string>();

            if (string.IsNullOrWhiteSpace(path))
            {
                failures.Add($"{configKey} is required.");
                return failures; // nothing else to validate
            }

            if (fileSystem.Directory.Exists(path))
            {
                failures.Add($"{configKey} must point to a file, not a directory.");
                return failures; // nothing else to validate
            }

            if (!fileSystem.Path.IsPathFullyQualified(path))
            {
                failures.Add($"{configKey} must be a fully qualified absolute path after normalisation. Path {path}");
            }

            var extension = fileSystem.Path.GetExtension(path);

            if (!string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
            {
                failures.Add($"{configKey} must point to a .json extension.");
            }

            if (!fileSystem.File.Exists(path))
            {
                failures.Add($"{configKey} must point to an existing file. File not found at {path}");
            }

            return failures;
        }
    }
}
