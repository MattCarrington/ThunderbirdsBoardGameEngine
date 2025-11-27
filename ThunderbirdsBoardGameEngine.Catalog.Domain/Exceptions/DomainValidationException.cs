namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    public abstract class DomainValidationException : Exception
    {
        protected DomainValidationException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
