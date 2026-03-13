using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source.Readers
{
    internal sealed class ExcelLocationReader
    {
        public List<LocationInput> ReadFrom(IXLWorksheet worksheet)
        {
            var headerRow = worksheet.FirstRowUsed();
            var columnMap = new ExcelMappingHelper(headerRow);

            var locations = new List<LocationInput>();

            var firstDataRow = headerRow.RowNumber() + 1;
            var lastRow = worksheet.LastRowUsed().RowNumber();

            var rows = worksheet.Rows(firstDataRow, lastRow);

            foreach (var row in rows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                locations.Add(new LocationInput(name));
            }

            return locations;
        }
    }
}