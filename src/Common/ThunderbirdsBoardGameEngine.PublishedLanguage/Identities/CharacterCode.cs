namespace ThunderbirdsBoardGameEngine.PublishedLanguage.Identities
{
    /// <summary>
    /// Represents a character code as an immutable value object.
    /// </summary>
    /// <param name="Value">The string value that identifies the character code.</param>
    public readonly record struct CharacterCode(string Value)
    {
        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the current object.</returns>
        public override string ToString() => Value;
    }
}
