using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.ComponentTests
{
    public static class InvalidFileTestCases
    {
        public static TheoryData<string, CatalogDataAccessErrorCode> InvalidFileCases =>
            new()
            {
                { "invalid-json.json", CatalogDataAccessErrorCode.BadJson },
                { "empty.json", CatalogDataAccessErrorCode.DataMissing }
            };

    }
}