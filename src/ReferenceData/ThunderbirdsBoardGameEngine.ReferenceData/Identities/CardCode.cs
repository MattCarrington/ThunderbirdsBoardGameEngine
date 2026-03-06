namespace ThunderbirdsBoardGameEngine.PublishedLanguage.Identities
{
    /// <summary>
    /// Represents an immutable card code value object.
    /// </summary>
    /// <param name="Value">The string value that represents the card code.</param>
    public readonly record struct CardCode(string Value)
    {
        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the current object.</returns>
        public override string ToString() => Value;
    }
}
