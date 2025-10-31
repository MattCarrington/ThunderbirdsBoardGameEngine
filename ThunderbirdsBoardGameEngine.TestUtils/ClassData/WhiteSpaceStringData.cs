using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.ClassData
{
    // Reusable xUnit ClassData for null/empty/whitespace strings
    public sealed class WhiteSpaceStringData : TheoryData<string>
    {
        public WhiteSpaceStringData()
        {
            Add(string.Empty);
            Add("");
            Add(" ");
            Add("\t");
            Add("\r");
            Add("\n");
            Add("\r\n");
            Add("   ");
        }
    }
}
