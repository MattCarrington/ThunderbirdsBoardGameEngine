namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    /// <summary>
    /// Represents a failure while accessing catalog data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception is thrown when an infrastructure or persistence error
    /// occurs while retrieving catalog data and is translated into a
    /// stable, application-level failure.
    /// </para>
    /// <para>
    /// The associated <see cref="CatalogDataAccessErrorCode"/> can be used by
    /// callers to distinguish between failure types without relying on
    /// infrastructure-specific details.
    /// </para>
    /// </remarks>
    public sealed class CatalogDataAccessException : Exception
    {
        /// <summary>
        /// Gets the error code describing the category of data access failure.
        /// </summary>
        public CatalogDataAccessErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the path or identifier of the catalog data source involved in the failure.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This value is intended for diagnostic and logging purposes only.
        /// </para>
        /// <para>
        /// Callers should not rely on the presence, format, or stability of this value
        /// for application logic.
        /// </para>
        /// </remarks>
        public string Path { get; }

        public CatalogDataAccessException(CatalogDataAccessErrorCode errorCode, string path, string? message, Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Path = path;
        }

        /// <summary>
        /// Creates an exception representing an unknown error with reading the catalog data.
        /// </summary>
        public static CatalogDataAccessException Unknown(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.Unknown, path, $"Unknown error reading catalog data from {path}", innerException);
        }

        /// <summary>
        /// Creates an exception representing missing catalog data.
        /// </summary>
        public static CatalogDataAccessException DataMissing(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.DataMissing, path, $"Missing required catalog data in {path}", innerException);
        }

        /// <summary>
        /// Creates an exception representing missing source of catalog data.
        /// </summary>
        public static CatalogDataAccessException SourceNotFound(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.SourceNotFound, path, $"Missing catalog data file: {path}", innerException);
        }

        /// <summary>
        /// Creates an exception representing invalid catalog data.
        /// </summary>
        public static CatalogDataAccessException BadJson(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.BadJson, path, $"Invalid JSON in {path}", innerException);
        }

        /// <summary>
        /// Creates an exception representing access to the catalog data source being denied.
        /// </summary>

        public static CatalogDataAccessException AccessDenied(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.AccessDenied, path, $"Access denied to catalog data file: {path}", innerException);
        }

        /// <summary>
        /// Creates an exception representing unavailable catalog data source.
        /// </summary>
        public static CatalogDataAccessException SourceUnreadable(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.SourceUnreadable, path, $"Unable to read catalog data file: {path}", innerException);
        }
    }

    /// <summary>
    /// Defines categories of catalog data access failures.
    /// </summary>
    /// <remarks>
    /// These error codes are stable and intended to be used for
    /// error handling and testing without relying on exception messages.
    /// </remarks>
    public enum CatalogDataAccessErrorCode
    {
        Unknown = 0,
        SourceNotFound,
        BadJson,
        AccessDenied,
        SourceUnreadable,
        DataMissing
    }
}
