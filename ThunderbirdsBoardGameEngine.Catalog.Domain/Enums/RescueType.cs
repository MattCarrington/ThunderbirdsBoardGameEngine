namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Enums
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
        Air,
        Land,
        Sea,
        Space
    }
}
