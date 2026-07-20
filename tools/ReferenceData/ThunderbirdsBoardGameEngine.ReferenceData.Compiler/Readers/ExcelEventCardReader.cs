using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelEventCardReader
    {
        public List<EventCardInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTableHelper.From(worksheet);
            var columnMap = table.Columns;

            var eventCards = new List<EventCardInput>();

            foreach (var row in table.DataRows)
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
