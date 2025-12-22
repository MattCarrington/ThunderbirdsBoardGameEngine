using ClosedXML.Excel;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;

public class ExcelHeaderMap
{
    private readonly Dictionary<string, int> _columnMap = new();

    public ExcelHeaderMap(IXLRow headerRow)
    {
        foreach (var cell in headerRow.CellsUsed())
        {
            var header = cell.GetString().Trim().ToLowerInvariant();
            _columnMap[header] = cell.Address.ColumnNumber;
        }
    }

    public int this[string columnName]
    {
        get
        {
            var key = columnName.Trim().ToLowerInvariant();
            if (!_columnMap.TryGetValue(key, out var col))
                throw new KeyNotFoundException($"Header '{columnName}' not found.");
            return col;
        }
    }

    public bool HasColumn(string columnName)
    {
        return _columnMap.ContainsKey(columnName.Trim().ToLowerInvariant());
    }
}
