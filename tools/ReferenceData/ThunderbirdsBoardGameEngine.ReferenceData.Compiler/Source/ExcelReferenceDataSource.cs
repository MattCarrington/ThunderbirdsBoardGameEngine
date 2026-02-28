namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source
{
    using ClosedXML.Excel;
    using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
    using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;

    public sealed class ExcelReferenceDataSource
    {
        private readonly string _path;

        public ExcelReferenceDataSource(string path)
        {
            _path = path;
        }

        public CompilationContext Load()
        {
            using var workbook = new XLWorkbook(_path);
            var worksheet = workbook.Worksheet(1);

            var headerRow = worksheet.FirstRowUsed();
            var columnMap = new ExcelMappingHelper(headerRow);

            var disasters = new List<DisasterInput>();

            var firstDataRow = headerRow.RowNumber() + 1;
            var lastRow = worksheet.LastRowUsed().RowNumber();

            var rows = worksheet.Rows(firstDataRow, lastRow);

            foreach (var row in rows)
            {
                var name = row.Cell(columnMap["Name"]).GetString();
                var difficulty = row.Cell(columnMap["Difficulty Number"]).GetValue<int>();
                var location = row.Cell(columnMap["Location"]).GetString();
                var rescueType = row.Cell(columnMap["Rescue Type"]).GetString();

                var bonuses = new List<BonusInput>();
                for (int i = 1; i <= 3; i++)
                {
                    var bonus = ReadBonus(row, columnMap, i);
                    if (bonus is not null)
                    {
                        bonuses.Add(bonus);
                    }
                }

                var rewards = new List<string>();
                for (int i = 1; i <= 2; i++)
                {
                    var reward = ReadReward(row, columnMap, i);
                    if (reward is not null)
                    {
                        rewards.Add(reward);
                    }
                }

                disasters.Add(new DisasterInput(
                    name,
                    difficulty,
                    location,
                    rescueType,
                    bonuses,
                    rewards));
            }

            return new CompilationContext
            {
                Disasters = disasters
            };
        }

        private static BonusInput? ReadBonus(IXLRow row, ExcelMappingHelper columnMap, int bonusNumber)
        {
            var targetCol = $"Bonus {bonusNumber}";
            var valueCol = $"Bonus {bonusNumber} Value";
            var locationCol = $"Bonus {bonusNumber} Location";

            if (!columnMap.HasColumn(targetCol))
            {
                return null;
            }

            var target = row.Cell(columnMap[targetCol]).GetString();
            if (string.IsNullOrWhiteSpace(target))
            {
                return null;
            }

            var value = row.Cell(columnMap[valueCol]).GetValue<int>();
            var location = columnMap.HasColumn(locationCol)
                ? row.Cell(columnMap[locationCol]).GetString()
                : string.Empty;

            return new BonusInput(
                target,
                value,
                string.IsNullOrWhiteSpace(location) ? null : location);
        }

        private static string? ReadReward(IXLRow row, ExcelMappingHelper columnMap, int rewardNumber)
        {
            var columnName = $"Reward {rewardNumber}";

            if (!columnMap.HasColumn(columnName))
            {
                return null;
            }

            var reward = row.Cell(columnMap[columnName]).GetString();

            if (string.IsNullOrWhiteSpace(reward))
            {
                return null;
            }

            return reward;
        }
    }
}
