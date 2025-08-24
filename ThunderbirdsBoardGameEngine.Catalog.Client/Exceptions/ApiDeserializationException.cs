namespace ThunderbirdsBoardGameEngine.Catalog.Client.Exceptions
{
    public class ApiDeserializationException : ApiClientException
    {
        public string? RawContent { get; }

        public ApiDeserializationException(string message, string? rawContent = null)
            : base(message) => RawContent = rawContent;
    }
}