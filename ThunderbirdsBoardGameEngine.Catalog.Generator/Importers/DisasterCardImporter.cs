using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Mappers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Importers
{
    public class DisasterCardImporter
    {
        public List<DisasterCardCatalogDto> ImportDisasterCardData(string path)
        {
            var result = new List<DisasterCardCatalogDto>();

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet(1); // First sheet
            var header = new ExcelHeaderMap(sheet.Row(1));

            var id = 0;

            foreach (var row in sheet.RowsUsed().Skip(1)) // Skip header
            {
                var name = Get(row, header, "Name");
                var code = StringHelpers.Slugify(name);
                var difficultyNumber = int.Parse(Get(row, header, "Difficulty Number"));
                var location = Get(row, header, "Location");
                var rescueType = Get(row, header, "Rescue Type");

                var bonuses = ParseBonuses(row, header);
                var rewards = ParseRewards(row, header);

                result.Add(new DisasterCardCatalogDto
                {
                    Id = id++,
                    Name = StringHelpers.NormalizeWhitespace(name, nameof(name)),
                    Code = code,
                    DifficultyNumber = difficultyNumber,
                    RescueType = StringHelpers.RemoveSpaces(rescueType),
                    Location = StringHelpers.RemoveSpaces(location),
                    BonusConditions = bonuses,
                    RewardOptions = rewards
                });
            }

            return result;
        }

        private static List<RewardOptionCatalogDto> ParseRewards(IXLRow row, ExcelHeaderMap header)
        {
            var rewards = new List<RewardOptionCatalogDto>();

            for (var i = 1; i < 2; i++)
            {
                var rewardKey = $"Reward {i}";
                if (!header.HasColumn(rewardKey)) continue;

                var value = Get(row, header, rewardKey);

                if (!string.IsNullOrWhiteSpace(value))
                {
                    var reward = RewardOptionCatalogDtoMapper.MapReward(value);
                    rewards.Add(reward);
                }
            }

            return rewards;
        }

        private static List<BonusConditionCatalogDto> ParseBonuses(IXLRow row, ExcelHeaderMap header)
        {
            var bonuses = new List<BonusConditionCatalogDto>();

            for (int i = 1; i <= 3; i++)
            {
                var bonusKey = $"Bonus {i}";

                if (header.HasColumn(bonusKey))
                {
                    var target = Get(row, header, bonusKey);

                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        var value = header.HasColumn($"{bonusKey} Value")
                            ? row.Cell(header[$"{bonusKey} Value"]).GetValue<int>()
                            : 1;

                        var location = header.HasColumn($"{bonusKey} Location")
                            ? Get(row, header, $"{bonusKey} Location")
                            : null;

                    
                        var bonus = BonusConditionCatalogDtoMapper.MapBonus(target, value, location);
                        bonuses.Add(bonus);
                    }
                }
            }

            return bonuses;
        }

        private static string Get(IXLRow row, ExcelHeaderMap header, string name)
        {
            return row.Cell(header[name]).GetString().Trim();
        }
    }
}