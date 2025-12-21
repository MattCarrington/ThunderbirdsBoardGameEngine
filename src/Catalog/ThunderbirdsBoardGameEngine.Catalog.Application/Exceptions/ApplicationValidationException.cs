namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    /// <summary>
    /// Represents a validation failure within the application layer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exception is thrown when one or more validation rules are violated
    /// while executing an application use case.
    /// </para>
    /// <para>
    /// It is intended to be caught by the API layer and translated into a
    /// client-visible validation error response (for example, HTTP 400).
    /// </para>
    /// </remarks>
    public sealed class ApplicationValidationException : Exception
    {
        /// <summary>
        /// Gets the collection of validation errors that caused the exception.
        /// </summary>
        /// <remarks>
        /// Each entry represents a discrete validation failure. The contents
        /// are safe to expose to API consumers.
        /// </remarks>
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationValidationException"/> class.
        /// </summary>
        /// <param name="errors">
        /// The validation errors that caused the operation to fail.
        /// </param>
        public ApplicationValidationException(
            string message,
            IReadOnlyDictionary<string, string[]>? errors = null) : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}
