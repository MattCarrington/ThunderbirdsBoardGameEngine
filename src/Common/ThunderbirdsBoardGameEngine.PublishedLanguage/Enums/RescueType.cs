namespace ThunderbirdsBoardGameEngine.PublishedLanguage.Enums
{
    /// <summary>
    /// Defines the type of rescue required to resolve a disaster.
    /// </summary>
    /// <remarks>
    /// The rescue type influences gameplay rules and may grant additional
    /// bonuses depending on the active character.
    /// </remarks>
    public enum RescueType
    {
        /// <summary>
        /// Represents the air rescue type for disasters.
        /// </summary>
        Air,
        /// <summary>
        /// Represents the land rescue type for disasters.
        /// </summary>
        Land,
        /// <summary>
        /// Represents the sea rescue type for disasters.
        /// </summary>
        Sea,
        /// <summary>
        /// Represents the space rescue type for disasters.
        /// </summary>
        Space
    }
}
