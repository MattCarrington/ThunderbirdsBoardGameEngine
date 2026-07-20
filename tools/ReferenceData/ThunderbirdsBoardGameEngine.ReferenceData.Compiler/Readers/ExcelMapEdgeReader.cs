using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers
{
    internal sealed class ExcelMapEdgeReader
    {
        public List<MapEdgeInput> ReadFrom(IXLWorksheet worksheet)
        {
            var table = ExcelWorksheetTableHelper.From(worksheet);
            var columnMap = table.Columns;

            var edges = new List<MapEdgeInput>();

            foreach (var row in table.DataRows)
            {
                var edge1 = row.Cell(columnMap["Edge 1"]).GetString();
                var edge2 = row.Cell(columnMap["Edge 2"]).GetString();

                if (string.IsNullOrWhiteSpace(edge1) || string.IsNullOrWhiteSpace(edge2))
                {
                    continue;
                }

                var domain = row.Cell(columnMap["Domain"]).GetString();

                edges.Add(new MapEdgeInput(edge1, edge2, domain));
            }

            return edges;
        }
    }
}
