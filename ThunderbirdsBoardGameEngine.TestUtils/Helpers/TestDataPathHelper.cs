namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    public static class TestDataPathHelper
    {
        public static string GetPath(string fileName, string subfolder = TestDataConstants.InputFolder)
        {
            Console.WriteLine($"Starting path: {AppContext.BaseDirectory}");

            // Navigate up until we find the solution root (heuristic: look for .sln file)
            var dir = AppContext.BaseDirectory;

            while (dir != null && !Directory.EnumerateFiles(dir, "*.sln").Any())
            {
                dir = Directory.GetParent(dir)?.FullName;
            }

            if (dir == null)
                throw new DirectoryNotFoundException("Could not find solution root from AppContext.BaseDirectory.");

            var baseDir = Path.Combine(dir, TestDataConstants.TestDataFolder, subfolder);
            var fullPath = Path.Combine(baseDir, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Expected test data not found at: {fullPath}");

            return fullPath;
        }
    }
}
