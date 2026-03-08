namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents a unique key that identifies a disaster bonus entry.
    /// </summary>
    public readonly record struct DisasterBonusKey
    {
        /// <summary>
        /// Gets the string value that uniquely identifies the disaster bonus.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the DisasterBonusKey struct with the specified value.
        /// </summary>
        /// <param name="value">The string value that uniquely identifies the disaster bonus. Cannot be null or whitespace.</param>
        public DisasterBonusKey(string value)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(value);
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
