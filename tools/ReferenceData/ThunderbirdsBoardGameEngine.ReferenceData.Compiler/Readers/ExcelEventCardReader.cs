using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelEventCardReader
    {
        public List<EventCardInput> ReadFrom(IXLWorksheet worksheet)
        {
            var headerRow = worksheet.FirstRowUsed();
            var columnMap = new ExcelMappingHelper(headerRow);

            var eventCards = new List<EventCardInput>();

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

                eventCards.Add(new EventCardInput(name));
            }

            return eventCards;
        }
    }
}