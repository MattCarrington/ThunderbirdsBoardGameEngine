namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    public static class TestDataPathHelper
    {
        public static string GetPath(string fileName)
        {
            var baseDir = Path.Combine(AppContext.BaseDirectory, "TestData");

            if (!Directory.Exists(baseDir))
            {
                // fallback or debug info
                throw new DirectoryNotFoundException($"Test data directory not found at {baseDir}");
            }

            return Path.Combine(baseDir, fileName);
        }
    }
}
