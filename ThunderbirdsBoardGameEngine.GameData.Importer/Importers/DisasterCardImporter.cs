using ClosedXML.Excel;
using GameDataImporter.ConsoleApp.Helpers;
using GameDataImporter.ConsoleApp.Parsers;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Importer.Parsers;
using ThunderbirdsBoardGameEngine.Serialization.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Importer.Importers
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

                var card = new DisasterCard
                {
                    Id = id++,
                    Name = row.Cell(header["Name"]).GetString(),
                    DifficultyNumber = row.Cell(header["Difficulty Number"]).GetValue<int>(),
                    Location = EnumDisplayHelper.ParseFromDisplayName<BoardLocation>(row.Cell(header["Location"]).GetString()),
                    RescueType = EnumDisplayHelper.ParseFromDisplayName<RescueType>(row.Cell(header["Rescue Type"]).GetString()),
                    BonusConditions = bonuses,
                    RewardOptions = rewards
                };

                cards.Add(card);
            }

            return cards;
        }
    }
}