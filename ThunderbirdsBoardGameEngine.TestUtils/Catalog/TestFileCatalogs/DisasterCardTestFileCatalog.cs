using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs
{
    public static class DisasterCardTestFileCatalog
    {
        public static string DataOnly(string fileName)
        {
            return TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.DisasterCardFolder,
                TestDataConstants.DataOnlyFolder,
                fileName);
        }

        public static string Enveloped(string fileName)
        {
            return TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.DisasterCardFolder,
                TestDataConstants.EnvelopedFolder,
                fileName);
        }

        public static string Invalid(string fileName)
        {
            return TestDataLocator.ExistingFile(
                TestDataConstants.CatalogFolder,
                TestDataConstants.DisasterCardFolder,
                TestDataConstants.InvalidFolder,
                fileName);
        }
    }

    // Folder structure to implement:
    // TestData
    //   └── Catalogs
    //         └── DisasterCard
    //               └── Data
    //               └── Enveloped
    //               └── Invalid
}
