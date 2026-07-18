using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mapper
{
    public class DisasterCardMapper
    {
        private readonly LocationCodeResolver _locationCodeResolver;
        private readonly DisasterBonusTargetResolver _disasterBonusTargetResolver;

        public DisasterCardMapper(LocationCodeResolver locationCodeResolver, DisasterBonusTargetResolver disasterBonusTargetResolver)
        {
            _locationCodeResolver = locationCodeResolver;
            _disasterBonusTargetResolver = disasterBonusTargetResolver;
        }

        public IEnumerable<ReferenceDisasterDefinition> Map(
            IEnumerable<DisasterInput> inputs)
        {
            return inputs
                .Select(MapDisaster);

        }

        private ReferenceDisasterDefinition MapDisaster(DisasterInput input)
        {
            return new ReferenceDisasterDefinition(
                new CardCode(StringHelpers.Slugify(input.Name)),
                StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)),
                input.DifficultyNumber,
                _locationCodeResolver.Resolve(input.Location),
                Enum.Parse<RescueType>(input.RescueType, true),
                MapBonuses(input.Bonuses).ToList(),
                MapRewards(input.Rewards).ToList());
        }

        private IEnumerable<ReferenceDisasterBonus> MapBonuses(IEnumerable<BonusInput> bonuses)
        {
            return bonuses.Select(b =>
                new ReferenceDisasterBonus(
                    _disasterBonusTargetResolver.Resolve(b.TargetName),
                    b.Value,
                    b.Location is null ? null : _locationCodeResolver.Resolve(b.Location)
                )
            );
        }

        private static IEnumerable<ReferenceDisasterReward> MapRewards(IEnumerable<string> rewards)
        {
            return rewards.Select(r =>
                r.Equals("User Choice", StringComparison.OrdinalIgnoreCase)
                    ? (ReferenceDisasterReward)new ReferenceDisasterReward.PlayerChoice()
                    : new ReferenceDisasterReward.SpecificToken(
                        Enum.Parse<BonusToken>(r, true))
            );
        }
    }
}
