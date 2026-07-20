using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelLocationReader
    {
        public List<LocationInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTable.From(worksheet);
            var columnMap = table.Columns;

            var locations = new List<LocationInput>();

            foreach (var row in table.DataRows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var domain = row.Cell(columnMap["Domain"]).GetString();

                locations.Add(new LocationInput(name, domain));
            }

            return locations;
        }
    }
}
