using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs
{
    public static class CharacterDefinitionTestFileCatalog
    {
        public static string Data(string fileName)
        {
            return TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.CharacterDefinitionFolder,
                fileName);
        }
    }
}
