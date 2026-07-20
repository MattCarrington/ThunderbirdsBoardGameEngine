using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelFabCardReader
    {
        public List<FabCardInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTable.From(worksheet);
            var columnMap = table.Columns;

            var fabCards = new List<FabCardInput>();

            foreach (var row in table.DataRows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                fabCards.Add(new FabCardInput(name));
            }

            return fabCards;
        }
    }
}
