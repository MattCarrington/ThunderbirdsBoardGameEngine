using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelPodVehicleReader
    {
        public List<PodVehicleInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTable.From(worksheet);
            var columnMap = table.Columns;

            var podVehicles = new List<PodVehicleInput>();

            foreach (var row in table.DataRows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();

                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                podVehicles.Add(new PodVehicleInput(name));
            }

            return podVehicles;
        }
    }
}
