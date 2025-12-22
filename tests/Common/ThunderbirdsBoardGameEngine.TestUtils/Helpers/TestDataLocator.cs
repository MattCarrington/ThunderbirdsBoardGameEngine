namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    internal static class TestDataLocator
    {
        private static readonly string TestDataRoot =
            Path.Combine(AppContext.BaseDirectory, TestDataConstants.TestDataFolder);

        /// <summary>
        /// Builds a path under TestData and verifies the file exists.
        /// </summary>
        public static string ExistingFile(params string[] segments)
        {
            var fullPath = Path.Combine(TestDataRoot, Path.Combine(segments));

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"Expected test data not found at: {fullPath}", fullPath);
            }

            return fullPath;
        }
    }
}