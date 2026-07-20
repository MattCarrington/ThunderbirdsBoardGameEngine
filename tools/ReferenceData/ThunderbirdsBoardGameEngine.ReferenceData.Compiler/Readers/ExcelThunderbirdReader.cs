using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelThunderbirdReader
    {
        public List<ThunderbirdInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTableHelper.From(worksheet);
            var columnMap = table.Columns;

            var thunderbirds = new List<ThunderbirdInput>();

            foreach (var row in table.DataRows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                var topSpeed = row.Cell(columnMap["Top Speed"]).GetValue<int>();

                var movementDomain = row.Cell(columnMap["Domain"]).GetString();

                thunderbirds.Add(new ThunderbirdInput(name, topSpeed, movementDomain));
            }

            return thunderbirds;
        }
    }
}
