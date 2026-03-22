namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents a location code as an immutable value object.
    /// </summary>
    public sealed record LocationCode
    {
        /// <summary>
        /// Gets the string value that identifies the location code.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the LocationCode struct with the specified value.
        /// </summary>
        /// <param name="value">The string value that identifies the location code. Cannot be null or whitespace.</param>
        public LocationCode(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
