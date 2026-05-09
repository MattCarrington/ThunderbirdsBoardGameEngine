namespace ThunderbirdsBoardGameEngine.PublishedLanguage.Characters
{
    /// <summary>
    /// Published identifier for a playable character.
    /// Represents a closed set of known values at system boundaries.
    /// </summary>
    [Obsolete("Published language is deprecated. Use Reference Data instead")]
    public readonly record struct CharacterCode
    {
        private CharacterCode(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the string value represented by this instance.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Returns the string representation of the current object.
        /// </summary>
        /// <returns>The value of the object as a string.</returns>
        public override string ToString() => Value;

        /// <summary>
        /// Represents the character code for Scott.
        /// </summary>
        public static readonly CharacterCode Scott = new("scott");

        /// <summary>
        /// Represents the character code for Virgil.
        /// </summary>
        public static readonly CharacterCode Virgil = new("virgil");

        /// <summary>
        /// Represents the character code for Alan.
        /// </summary>
        public static readonly CharacterCode Alan = new("alan");

        /// <summary>
        /// Represents the character code for Gordon.
        /// </summary>
        public static readonly CharacterCode Gordon = new("gordon");

        /// <summary>
        /// Represents the character code for John.
        /// </summary>
        public static readonly CharacterCode John = new("john");

        /// <summary>
        /// Represents the character code for Lady Penelope.
        /// </summary>
        public static readonly CharacterCode LadyPenelope = new("lady-penelope");

        private static readonly Dictionary<string, CharacterCode> Lookup =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["scott"] = Scott,
                ["virgil"] = Virgil,
                ["alan"] = Alan,
                ["gordon"] = Gordon,
                ["john"] = John,
                ["lady-penelope"] = LadyPenelope,
                ["ladypenelope"] = LadyPenelope // optional alias
            };

        /// <summary>
        /// Attempts to parse the specified input string into a corresponding CharacterCode value.
        /// </summary>
        /// <remarks>This method does not throw an exception if parsing fails. Use this method to safely
        /// attempt parsing when the input may be invalid or unknown.</remarks>
        /// <param name="input">The input string to parse. Leading and trailing whitespace are ignored. Can be null or empty.</param>
        /// <param name="code">When this method returns, contains the parsed CharacterCode value if parsing succeeded; otherwise, contains
        /// the default value.</param>
        /// <returns>true if the input string was successfully parsed into a CharacterCode value; otherwise, false.</returns>
        public static bool TryParse(string? input, out CharacterCode code)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                code = default;
                return false;
            }

            return Lookup.TryGetValue(input.Trim(), out code);
        }
    }
}
