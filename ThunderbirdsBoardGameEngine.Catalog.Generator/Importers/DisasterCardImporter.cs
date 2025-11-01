using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Generator.Parsers;

namespace ThunderbirdsBoardGameEngine.Catalog.Generator.Importers
{
    public class DisasterCardImporter
    {
        public List<DisasterCard> Import(string path)
        {
            var cards = new List<DisasterCard>();

            using var workbook = new XLWorkbook(path);
            var sheet = workbook.Worksheet(1); // First sheet
            var header = new ExcelHeaderMap(sheet.Row(1));

            var id = 0;

            foreach (var row in sheet.RowsUsed().Skip(1)) // Skip header
            {
                var bonuses = new List<BonusCondition>();

                for (int i = 1; i <= 3; i++)
                {                  
                    var bonusKey = $"Bonus {i}";
                    if (!header.HasColumn(bonusKey)) break;

                    var target = row.Cell(header[bonusKey]).GetString();
                    var value = header.HasColumn($"{bonusKey} Value")
                        ? row.Cell(header[$"{bonusKey} Value"]).GetValue<int?>()
                        : 1;

                    var loc = header.HasColumn($"{bonusKey} Location")
                        ? row.Cell(header[$"{bonusKey} Location"]).GetString()
                        : null;

                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        var bonus = BonusParser.Parse(target, value ?? 1, loc);
                        bonuses.Add(bonus);
                    }
                }

                var rewards = new List<RewardOption>();

                for (int i = 1; i <= 2; i++)
                {
                    var rewardKey = $"Reward {i}";
                    if (!header.HasColumn(rewardKey)) continue;

                    var value = row.Cell(header[rewardKey]).GetString();

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        var reward = RewardParser.Parse(value);
                        rewards.Add(reward);
                    }
                }

                var card = new DisasterCard(
                    id++,
                    row.Cell(header["Name"]).GetString(),
                    row.Cell(header["Name"]).GetString().ToLowerInvariant().Replace(" ", "-"),
                    row.Cell(header["Difficulty Number"]).GetValue<int>(),
                    EnumDisplayHelper.ParseFromDisplayName<BoardLocation>(row.Cell(header["Location"]).GetString()),
                    EnumDisplayHelper.ParseFromDisplayName<RescueType>(row.Cell(header["Rescue Type"]).GetString()),
                    bonuses,
                    rewards
                );

                cards.Add(card);
            }

            return cards;
        }
    }
}