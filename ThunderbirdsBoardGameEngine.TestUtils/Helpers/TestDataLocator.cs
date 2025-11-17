namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    internal static class TestDataLocator
    {
        public static string TestDataRoot =>
            Path.Combine(SolutionRoot.Value, TestDataConstants.TestDataFolder);

        /// <summary>
        /// New-style: build a path under TestData from segments.
        /// </summary>
        public static string ExistingFile(params string[] segments)
        {
            var fullPath = Path.Combine(TestDataRoot, Path.Combine(segments));

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Expected test data not found at: {fullPath}", fullPath);
            }

            return fullPath;
        }

        private static readonly Lazy<string> SolutionRoot = new(() =>
        {
            var dir = AppContext.BaseDirectory;

            while (dir != null && !Directory.EnumerateFiles(dir, "*.sln").Any())
            {
                dir = Directory.GetParent(dir)?.FullName;
            }

            if (dir == null)
            {
                throw new DirectoryNotFoundException("Could not find solution root from AppContext.BaseDirectory.");
            }

            return dir;
        });
    }
}