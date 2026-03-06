namespace ThunderbirdsBoardGameEngine.PublishedLanguage.Identities
{
    /// <summary>
    /// Represents a Thunderbird code as an immutable value object.
    /// </summary>
    /// <param name="Value">The string value of the Thunderbird code.</param>
    public readonly record struct ThunderbirdCode(string Value)
    {
        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the current object.</returns>
        public override string ToString() => Value;
    }
}
