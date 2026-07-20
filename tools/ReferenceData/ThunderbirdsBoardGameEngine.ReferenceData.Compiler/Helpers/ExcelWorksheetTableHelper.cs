using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers
{
    public sealed class ExcelWorksheetTableHelper
    {
        private ExcelWorksheetTableHelper(
            ExcelMappingHelper columns,
            IReadOnlyList<IXLRow> dataRows)
        {
            Columns = columns;
            DataRows = dataRows;
        }

        public ExcelMappingHelper Columns { get; }

        public IReadOnlyList<IXLRow> DataRows { get; }

        public static ExcelWorksheetTableHelper From(IXLWorksheet worksheet)
        {
            ArgumentNullException.ThrowIfNull(worksheet);

            var headerRow = worksheet.FirstRowUsed()
                ?? throw new ReferenceDataCompilationException(
                    $"Worksheet '{worksheet.Name}' is empty and has no header row.");

            var firstDataRowNumber = headerRow.RowNumber() + 1;
            var lastUsedRowNumber = worksheet.LastRowUsed()?.RowNumber();

            IReadOnlyList<IXLRow> dataRows =
                lastUsedRowNumber is null || lastUsedRowNumber < firstDataRowNumber
                    ? []
                    : worksheet.Rows(firstDataRowNumber, lastUsedRowNumber.Value).ToList();

            return new ExcelWorksheetTableHelper(
                new ExcelMappingHelper(headerRow),
                dataRows);
        }
    }
}
