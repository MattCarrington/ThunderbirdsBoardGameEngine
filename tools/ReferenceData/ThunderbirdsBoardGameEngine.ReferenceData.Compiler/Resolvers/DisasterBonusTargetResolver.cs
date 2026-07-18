using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers
{
    public sealed class DisasterBonusTargetResolver
    {
        private readonly Dictionary<string, DisasterBonusKey> _bonusKeysByName;

        public DisasterBonusTargetResolver(
            IEnumerable<ReferenceCharacterDefinition> characters,
            IEnumerable<ReferencePodVehicleDefinition> podVehicles,
            IEnumerable<ReferenceThunderbirdDefinition> thunderbirds)
        {
            var targets = characters
                .Select(character => (
                    Name: character.DisplayName,
                    Key: new DisasterBonusKey(character.Code.Value),
                    Type: "Character"))
                .Concat(podVehicles.Select(podVehicle => (
                    Name: podVehicle.DisplayName,
                    Key: new DisasterBonusKey(podVehicle.Code.Value),
                    Type: "Pod Vehicle")))
                .Concat(thunderbirds.Select(thunderbird => (
                    Name: thunderbird.DisplayName,
                    Key: new DisasterBonusKey(thunderbird.Code.Value),
                    Type: "Thunderbird")))
                .ToList();

            var targetsByName = targets
                .GroupBy(target => target.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var ambiguousNames = targetsByName
                .Where(group => group.Count() > 1)
                .Select(group =>
                    $"{group.Key} ({string.Join(", ", group.Select(target => target.Type))})")
                .ToList();

            if (ambiguousNames.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    "Disaster bonus target names must be unique across characters, " +
                    "Thunderbirds, and pod vehicles. " +
                    $"Ambiguous targets: {string.Join("; ", ambiguousNames)}");
            }

            _bonusKeysByName = targetsByName.ToDictionary(
                group => group.Key,
                group => group.Single().Key,
                StringComparer.OrdinalIgnoreCase);
        }

        public DisasterBonusKey Resolve(string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
            {
                throw new ArgumentException("Disaster bonus target name cannot be null or whitespace.", nameof(targetName));
            }

            var normalizedName = StringHelpers.NormalizeWhitespace(targetName, nameof(targetName));

            if (_bonusKeysByName.TryGetValue(normalizedName, out var key))
            {
                return key;
            }

            throw new ReferenceDataCompilationException(
                $"Disaster bonus target '{targetName}' does not reference a character, Thunderbird, or pod vehicle.");
        }
    }
}
