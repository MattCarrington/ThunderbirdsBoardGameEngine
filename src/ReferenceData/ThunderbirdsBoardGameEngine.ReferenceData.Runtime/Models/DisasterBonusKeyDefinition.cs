using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models
{
    /// <summary>
    /// Represents a disaster bonus key definition in the reference data.
    /// </summary>
    /// <param name="Key">The unique disaster bonus key.</param>    
    /// <param name="DisplayName">The display name of the disaster bonus key.</param>
    public sealed record DisasterBonusKeyDefinition(
        DisasterBonusKey Key,
        string DisplayName
    );
}