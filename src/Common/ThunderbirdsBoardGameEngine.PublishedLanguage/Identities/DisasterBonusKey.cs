namespace ThunderbirdsBoardGameEngine.PublishedLanguage.Identities
{
    /// <summary>
    /// Represents a unique key that identifies a disaster bonus entry.
    /// </summary>
    /// <param name="Value">The string value that uniquely identifies the disaster bonus.</param>
    public readonly record struct DisasterBonusKey(string Value)
    {
        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>A string that represents the value of the current object.</returns>
        public override string ToString() => Value;
    }
}
