using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs
{
    public static class SharedTestFileCatalog
    {
        public static string Invalid(string filename)
        {
            return TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.SharedFolder,
                TestDataConstants.InvalidFolder,
                filename);
        }
    }
}
