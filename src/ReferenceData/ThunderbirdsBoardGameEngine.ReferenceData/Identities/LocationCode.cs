namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents a location code as an immutable value object.
    /// </summary>
    public readonly record struct LocationCode
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
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Location code cannot be null or whitespace.", nameof(value));
            }

            Value = value;
        }

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the current object.</returns>
        public override string ToString()
        {
            return Value;
        }
    }
}
