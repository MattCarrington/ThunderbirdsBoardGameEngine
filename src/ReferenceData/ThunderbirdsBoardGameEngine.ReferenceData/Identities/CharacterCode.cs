namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents a character code as an immutable value object.
    /// </summary>
    /// <param name="Value">The string value that identifies the character code.</param>
    public sealed record CharacterCode
    {
        /// <summary>
        /// Gets the string value that identifies the character code.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterCode"/> record.
        /// </summary>
        /// <param name="value">The string value that identifies the character code.</param>
        /// <exception cref="ArgumentException">Thrown when the value is null or whitespace.</exception>
        public CharacterCode(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            Value = value;
        }

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the current object.</returns>
        public override string ToString() => Value;
    }
}
