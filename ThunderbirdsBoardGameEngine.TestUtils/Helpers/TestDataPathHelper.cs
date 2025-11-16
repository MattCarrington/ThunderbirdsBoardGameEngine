namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    public static class TestDataPathHelper
    {
        public static string GetPath(string filepath)
        {
            return TestDataLocator.FromSolutionRelativePath(filepath);
        }
    }
}
