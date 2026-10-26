using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Models
{
    /// <summary>
    /// Represents a disaster bonus key definition in the reference data.
    /// </summary>
    public sealed record DisasterBonusKeyDefinition
    {
        /// <summary>Gets the unique disaster bonus key.</summary>
        public DisasterBonusKey Key { get; }

        /// <summary>Gets the display name of the bonus target.</summary>
        public string DisplayName { get; }

        internal DisasterBonusKeyDefinition(DisasterBonusKey key, string displayName)
        {
            Key = key;
            DisplayName = displayName;
        }

        /// <summary>Deconstructs the bonus target into its key and display name.</summary>
        /// <param name="Key">The unique disaster bonus key.</param>
        /// <param name="DisplayName">The display name of the bonus target.</param>
        public void Deconstruct(out DisasterBonusKey Key, out string DisplayName)
        {
            Key = this.Key;
            DisplayName = this.DisplayName;
        }
    }
}
