namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    public sealed class CatalogDataAccessException : Exception
    {
        public CatalogDataAccessErrorCode ErrorCode { get; }

        public string Path { get; }

        public CatalogDataAccessException(CatalogDataAccessErrorCode errorCode, string path, string? message, Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Path = path;
        }

        public static CatalogDataAccessException Unknown(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.Unknown, path, $"Unknown error reading catalog data from {path}", innerException);
        }

        public static CatalogDataAccessException DataMissing(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.DataMissing, path, $"Missing required catalog data in {path}", innerException);
        }

        public static CatalogDataAccessException SourceNotFound(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.SourceNotFound, path, $"Missing catalog data file: {path}", innerException);
        }

        public static CatalogDataAccessException BadJson(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.BadJson, path, $"Invalid JSON in {path}", innerException);
        }

        public static CatalogDataAccessException AccessDenied(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.AccessDenied, path, $"Access denied to catalog data file: {path}", innerException);
        }

        public static CatalogDataAccessException SourceUnreadable(string path, Exception? innerException = null)
        {
            return new CatalogDataAccessException(CatalogDataAccessErrorCode.SourceUnreadable, path, $"Unable to read catalog data file: {path}", innerException);
        }
    }

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
