using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData
{
    public class BomOnlyData : TheoryData<string>
    {
        public BomOnlyData()
        {
            Add("\uFEFF");
            Add("\uFEFF ");
            Add("\uFEFF\r\n\t  ");
        }
    }
}
