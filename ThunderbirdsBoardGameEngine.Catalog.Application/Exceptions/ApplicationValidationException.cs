namespace ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions
{
    public sealed class ApplicationValidationException : Exception
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ApplicationValidationException(
            string message,
            IReadOnlyDictionary<string, string[]>? errors = null) : base(message)
        {
            Errors = errors ?? new Dictionary<string, string[]>();
        }
    }
}
