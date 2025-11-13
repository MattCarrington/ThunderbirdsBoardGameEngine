using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData
{
    // Reusable xUnit ClassData for null/empty/whitespace strings
    public sealed class WhiteSpaceStringData : TheoryData<string>
    {
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
