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
        /// <summary>
        /// Gets or sets the type of resource represented by this instance.
        /// </summary>
        public string ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the associated resource.
        /// </summary>
        public object ResourceId { get; set; }

        protected NotFoundException(string message, string resourceType, object resourceId)
            : base(message)
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }
    }
}
