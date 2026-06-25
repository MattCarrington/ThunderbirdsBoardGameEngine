namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities
{
    /// <summary>
    /// Represents a Thunderbird code as an immutable value object.
    /// </summary>
    public readonly record struct ThunderbirdCode
    {
        /// <summary>
        /// Gets the string value that identifies the Thunderbird code.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThunderbirdCode"/> record.
        /// </summary>
        /// <param name="value">The string value that identifies the Thunderbird code.</param>
        /// <exception cref="ArgumentException">Thrown when the value is null or whitespace.</exception>
        public ThunderbirdCode(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

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
