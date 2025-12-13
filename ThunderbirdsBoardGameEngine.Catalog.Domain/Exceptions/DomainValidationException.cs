namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    /// <summary>
    /// Base type for domain-level validation failures.
    /// </summary>
    /// <remarks>
    /// Domain validation exceptions indicate that domain invariants or
    /// consistency rules have been violated.
    /// </remarks>
    public abstract class DomainValidationException : Exception
    {
        protected DomainValidationException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
