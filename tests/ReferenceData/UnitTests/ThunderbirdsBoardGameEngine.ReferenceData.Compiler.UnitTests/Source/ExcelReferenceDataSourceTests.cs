using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Source
{
    public sealed class ExcelReferenceDataSourceTests
    {
        [Fact]
        public void Load_WhenRequiredWorksheetIsMissing_ThrowsCompilationException()
        {
            var workbookPath = CreateWorkbook(workbook =>
                workbook.AddWorksheet("Unexpected Sheet"));

            try
            {
                var source = new ExcelReferenceDataSource(workbookPath);

                var exception = Assert.Throws<ReferenceDataCompilationException>(
                    source.Load);

                Assert.Contains("Required worksheet 'Disaster Cards' was not found", exception.Message);
            }
            finally
            {
                File.Delete(workbookPath);
            }
        }

        [Fact]
        public void Load_WhenRequiredWorksheetIsEmpty_ThrowsCompilationException()
        {
            var workbookPath = CreateWorkbook(workbook =>
                workbook.AddWorksheet("Disaster Cards"));

            try
            {
                var source = new ExcelReferenceDataSource(workbookPath);

                var exception = Assert.Throws<ReferenceDataCompilationException>(
                    source.Load);

                Assert.Contains("Worksheet 'Disaster Cards' is empty and has no header row", exception.Message);
            }
            finally
            {
                File.Delete(workbookPath);
            }
        }

        private static string CreateWorkbook(Action<XLWorkbook> configure)
        {
            var workbookPath = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.xlsx");

            using var workbook = new XLWorkbook();
            configure(workbook);
            workbook.SaveAs(workbookPath);

            return workbookPath;
        }
    }
}
