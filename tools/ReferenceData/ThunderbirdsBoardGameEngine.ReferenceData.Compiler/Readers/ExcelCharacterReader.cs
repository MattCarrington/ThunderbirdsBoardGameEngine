using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelCharacterReader
    {
        public List<CharacterInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTableHelper.From(worksheet);
            var columnMap = table.Columns;

            var characters = new List<CharacterInput>();

            foreach (var row in table.DataRows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                // Rescue Type is optional (Lady Penelope case)
                string? rescueType = null;
                if (columnMap.HasColumn("Rescue Type"))
                {
                    rescueType = row.Cell(columnMap["Rescue Type"]).GetString();
                    if (string.IsNullOrWhiteSpace(rescueType))
                    {
                        rescueType = null;
                    }
                }

                // Value is optional (Lady Penelope case)
                int? value = null;
                if (columnMap.HasColumn("Value"))
                {
                    var valueCell = row.Cell(columnMap["Value"]);
                    if (!valueCell.IsEmpty())
                    {
                        value = valueCell.GetValue<int>();
                    }
                }

                characters.Add(new CharacterInput(name, rescueType, value));
            }

            return characters;
        }
    }
}
