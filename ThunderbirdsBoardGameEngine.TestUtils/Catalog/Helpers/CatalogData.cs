using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers
{
    public static class CatalogData
    {
        public static string HttpRequest(string scenarioName, int version = 1) =>
            TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.InputFolder,
                TestDataConstants.HttpFolder,
                $"V{version}",
                $"{scenarioName}.json");

        public static string HttpExpectedResponse(string scenarioName, int version = 1) =>
            TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.ExpectedFolder,
                TestDataConstants.HttpFolder,
                $"V{version}",
                $"{scenarioName}.json");

        public static string FileInput(string fileName) =>
            TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.InputFolder,
                TestDataConstants.FileFolder,
                fileName);
    }
}
