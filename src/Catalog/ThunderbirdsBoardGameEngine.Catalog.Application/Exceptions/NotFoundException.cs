namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    /// <summary>
    /// Represents the base class for exceptions that are thrown when a requested entity cannot be found.
    /// </summary>
    /// <remarks>This exception is intended to be used as a base class for more specific not-found exception
    /// types. It enables distinguishing between different not-found scenarios in application logic or error handling.
    /// Typically, derived exceptions provide additional context about the missing entity.</remarks>
    public abstract class NotFoundException : Exception
    {
        protected NotFoundException(string message)
            : base(message)
        {
        }
    }
}
