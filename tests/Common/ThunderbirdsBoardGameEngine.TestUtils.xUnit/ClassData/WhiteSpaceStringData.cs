using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData
{
    /// <summary>
    /// Provides a selection of empty and whitespace-only strings for xUnit theories.
    /// </summary>
    public sealed class WhiteSpaceStringData : TheoryData<string>
    {
        /// <inheritdoc />
        public WhiteSpaceStringData()
        {
            Add(string.Empty);          // empty string
            Add(new string(' ', 1));    // single space
            Add(new string(' ', 10));   // whitespace
            Add("\t");                  // tab
            Add("\r");                  // carriage return
            Add("\n");                  // linefeed
            Add("\r\n");                // clrf
        }
    }
}
