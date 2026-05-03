namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents an immutable card code value object.
    /// </summary>
    public readonly record struct CardCode
    {
        /// <summary>
        /// Gets the string value represented by this instance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the CardCode class with the specified card code value.
        /// </summary>
        /// <param name="value">The card code value to assign. Cannot be null or empty.</param>
        public CardCode(string value)
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
