namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents a disaster bonus key as an immutable value object.
    /// </summary>
    public readonly record struct DisasterBonusKey
    {
        /// <summary>
        /// Gets the string value that identifies the disaster bonus key.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DisasterBonusKey"/> record.
        /// </summary>
        /// <param name="value">The string value that identifies the disaster bonus key.</param>
        /// <exception cref="ArgumentException">Thrown when the value is null or whitespace.</exception>
        public DisasterBonusKey(string value)
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
