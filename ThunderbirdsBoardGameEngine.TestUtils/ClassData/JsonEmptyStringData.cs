using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.ClassData
{
    public sealed class JsonEmptyStringData : TheoryData<string?>
    {
        public JsonEmptyStringData()
        {
            foreach (var item in new WhiteSpaceStringData())
            {
                Add(item);
            }

            // UTF-8 BOM only or BOM + whitespace
            Add("\uFEFF");
            Add("\uFEFF ");
            Add("\uFEFF\r\n\t  ");

            // Big whitespace-only (exercise peek window)
            Add(new string(' ', 8192));
            Add("\uFEFF" + new string('\n', 8192));
        }
    }
}
