using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions
{
    /// <summary>
    /// Provides assertion helpers for verifying exceptions related to catalog data access operations in unit tests.
    /// </summary>
    /// <remarks>This class is intended for use in test code to validate that catalog data access methods
    /// throw the expected exceptions with the correct error codes and inner exception types. It is not intended for use
    /// in production code.</remarks>
    public static class CatalogDataAccessAssertions
    {
        /// <summary>
        /// Asserts that the specified asynchronous action throws a CatalogDataAccessException with the expected error
        /// code, path, and inner exception type.
        /// </summary>
        /// <remarks>Use this method in unit tests to verify that an operation fails with a
        /// CatalogDataAccessException containing the specified error code, path, and inner exception type.</remarks>
        /// <typeparam name="TInner">The type of the expected inner exception. Must derive from Exception.</typeparam>
        /// <param name="action">The asynchronous action expected to throw a CatalogDataAccessException.</param>
        /// <param name="expectedErrorCode">The expected error code value of the thrown CatalogDataAccessException.</param>
        /// <param name="path">The expected path value of the thrown CatalogDataAccessException.</param>
        /// <returns>A task that represents the asynchronous assertion operation.</returns>
        public static async Task AssertCatalogDataAccessException<TInner>(Func<Task> action, CatalogDataAccessErrorCode expectedErrorCode, string path) where TInner : Exception
        {
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(action);
            Assert.Equal(expectedErrorCode, exception.ErrorCode);
            Assert.Equal(path, exception.Path);
            Assert.IsType<TInner>(exception.InnerException);
        }
    }
}
