using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Resolvers
{
    public class DisasterBonusTargetResolverTests
    {
        [Fact]
        public void Resolve_WhenCharacterNameExists_ShouldReturnCorrectDisasterBonusKey()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve("Character A");

            // Assert
            Assert.Equal(new DisasterBonusKey("CHAR_A"), result);
        }

        [Fact]
        public void Resolve_WhenThunderbirdNameExists_ShouldReturnCorrectDisasterBonusKey()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve("Thunderbird 1");

            // Assert
            Assert.Equal(new DisasterBonusKey("TB1"), result);
        }

        [Fact]
        public void Resolve_WhenPodVehicleNameExists_ShouldReturnCorrectDisasterBonusKey()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve("Pod Vehicle A");

            // Assert
            Assert.Equal(new DisasterBonusKey("PV_A"), result);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Resolve_WhenTargetNameIsNullOrWhitespace_ShouldThrowArgumentException(string? targetName)
        {
            // Arrange
            var resolver = CreateResolver();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => resolver.Resolve(targetName));
            Assert.Equal("Disaster bonus target name cannot be null or whitespace. (Parameter 'targetName')", exception.Message);
        }

        [Fact]
        public void Resolve_WhenDisasterBonusTargetNameDoesNotExist_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => resolver.Resolve("Nonexistent Disaster Bonus Target"));
            Assert.Equal("Disaster bonus target 'Nonexistent Disaster Bonus Target' does not reference a character, Thunderbird, or pod vehicle.", exception.Message);
        }

        [Fact]
        public void Resolve_WhenDisasterBonusTargetNameExistsInDifferentCase_ShouldReturnCorrectDisasterBonusKey()
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve("character a");

            // Assert
            Assert.Equal(new DisasterBonusKey("CHAR_A"), result);
        }

        [Theory]
        [InlineData("    Character A")]
        [InlineData("Character A    ")]
        [InlineData("  Character A  ")]
        public void Resolve_WhenCharacterNameExistsWithLeadingOrTrailingWhitespace_ShouldReturnCorrectDisasterBonusKey(string targetName)
        {
            // Arrange
            var resolver = CreateResolver();

            // Act
            var result = resolver.Resolve(targetName);

            // Assert
            Assert.Equal(new DisasterBonusKey("CHAR_A"), result);
        }

        [Fact]
        public void Constructor_WhenCharacterNamesAreNotUnique_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var characters = new List<ReferenceCharacterDefinition>
            {
                new(displayName: "Character A", code: new CharacterCode("CHAR_A"), rescueBonus: null),
                new(displayName: "Character A", code: new CharacterCode("CHAR_B"), rescueBonus: null)
            };

            var thunderbirds = new List<ReferenceThunderbirdDefinition>
            {
                new(displayName: "Thunderbird 1", code: new ThunderbirdCode("TB1"), topSpeed: 100, domain: MovementDomain.Earth),
                new(displayName: "Thunderbird 2", code: new ThunderbirdCode("TB2"), topSpeed: 120, domain: MovementDomain.Space)
            };

            var podVehicles = new List<ReferencePodVehicleDefinition>
            {
                new(displayName: "Pod Vehicle A", code: new PodVehicleCode("PV_A")),
                new(displayName: "Pod Vehicle B", code: new PodVehicleCode("PV_B"))
            };

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => new DisasterBonusTargetResolver(characters, podVehicles, thunderbirds));
            Assert.Equal(
                "Disaster bonus target names must be unique across characters, Thunderbirds, and pod vehicles. Ambiguous targets: Character A (Character, Character)",
                exception.Message);
        }

        [Fact]
        public void Constructor_WhenThunderbirdNamesAreNotUnique_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var characters = new List<ReferenceCharacterDefinition>
            {
                new(displayName: "Character A", code: new CharacterCode("CHAR_A"), rescueBonus: null),
                new(displayName: "Character B", code: new CharacterCode("CHAR_B"), rescueBonus: null)
            };

            var thunderbirds = new List<ReferenceThunderbirdDefinition>
            {
                new(displayName: "Thunderbird 1", code: new ThunderbirdCode("TB1"), topSpeed: 100, domain: MovementDomain.Earth),
                new(displayName: "Thunderbird 1", code: new ThunderbirdCode("TB2"), topSpeed: 120, domain: MovementDomain.Space)
            };

            var podVehicles = new List<ReferencePodVehicleDefinition>
            {
                new(displayName: "Pod Vehicle A", code: new PodVehicleCode("PV_A")),
                new(displayName: "Pod Vehicle B", code: new PodVehicleCode("PV_B"))
            };

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => new DisasterBonusTargetResolver(characters, podVehicles, thunderbirds));
            Assert.Equal(
                "Disaster bonus target names must be unique across characters, Thunderbirds, and pod vehicles. Ambiguous targets: Thunderbird 1 (Thunderbird, Thunderbird)",
                exception.Message);
        }

        [Fact]
        public void Constructor_WhenPodVehicleNamesAreNotUnique_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var characters = new List<ReferenceCharacterDefinition>
            {
                new(displayName: "Character A", code: new CharacterCode("CHAR_A"), rescueBonus: null),
                new(displayName: "Character B", code: new CharacterCode("CHAR_B"), rescueBonus: null)
            };

            var thunderbirds = new List<ReferenceThunderbirdDefinition>
            {
                new(displayName: "Thunderbird 1", code: new ThunderbirdCode("TB1"), topSpeed: 100, domain: MovementDomain.Earth),
                new(displayName: "Thunderbird 2", code: new ThunderbirdCode("TB2"), topSpeed: 120, domain: MovementDomain.Space)
            };

            var podVehicles = new List<ReferencePodVehicleDefinition>
            {
                new(displayName: "Pod Vehicle A", code: new PodVehicleCode("PV_A")),
                new(displayName: "Pod Vehicle A", code: new PodVehicleCode("PV_B"))
            };

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => new DisasterBonusTargetResolver(characters, podVehicles, thunderbirds));
            Assert.Equal(
                "Disaster bonus target names must be unique across characters, Thunderbirds, and pod vehicles. Ambiguous targets: Pod Vehicle A (Pod Vehicle, Pod Vehicle)",
                exception.Message);
        }

        [Fact]
        public void Constructor_WhenNamesAreNotUniqueAcrossTypes_ShouldThrowReferenceDataCompilationException()
        {
            // Arrange
            var characters = new List<ReferenceCharacterDefinition>
            {
                new(displayName: "Character A", code: new CharacterCode("CHAR_A"), rescueBonus: null),
                new(displayName: "Thunderbird 1", code: new CharacterCode("CHAR_B"), rescueBonus: null)
            };

            var thunderbirds = new List<ReferenceThunderbirdDefinition>
            {
                new(displayName: "Thunderbird 1", code: new ThunderbirdCode("TB1"), topSpeed: 100, domain: MovementDomain.Earth),
                new(displayName: "Pod Vehicle A", code: new ThunderbirdCode("TB2"), topSpeed: 120, domain: MovementDomain.Space)
            };

            var podVehicles = new List<ReferencePodVehicleDefinition>
            {
                new(displayName: "Pod Vehicle A", code: new PodVehicleCode("PV_A")),
                new(displayName: "Character A", code: new PodVehicleCode("PV_B"))
            };

            // Act & Assert
            var exception = Assert.Throws<ReferenceDataCompilationException>(() => new DisasterBonusTargetResolver(characters, podVehicles, thunderbirds));
            Assert.Contains(
                "Disaster bonus target names must be unique across characters, Thunderbirds, and pod vehicles. Ambiguous targets:",
                exception.Message);
            Assert.Contains("Character A (Character, Pod Vehicle)", exception.Message);
            Assert.Contains("Thunderbird 1 (Character, Thunderbird)", exception.Message);
            Assert.Contains("Pod Vehicle A (Pod Vehicle, Thunderbird)", exception.Message);
        }

        private static DisasterBonusTargetResolver CreateResolver()
        {
            var characters = new List<ReferenceCharacterDefinition>
            {
                new(displayName: "Character A", code: new CharacterCode("CHAR_A"), rescueBonus: null),
                new(displayName: "Character B", code: new CharacterCode("CHAR_B"), rescueBonus: null)
            };

            var thunderbirds = new List<ReferenceThunderbirdDefinition>
            {
                new(displayName: "Thunderbird 1", code: new ThunderbirdCode("TB1"), topSpeed: 100, domain: MovementDomain.Earth),
                new(displayName: "Thunderbird 2", code: new ThunderbirdCode("TB2"), topSpeed: 120, domain: MovementDomain.Space)
            };

            var podVehicles = new List<ReferencePodVehicleDefinition>
            {
                new(displayName: "Pod Vehicle A", code: new PodVehicleCode("PV_A")),
                new(displayName: "Pod Vehicle B", code: new PodVehicleCode("PV_B"))
            };

            return new DisasterBonusTargetResolver(characters, podVehicles, thunderbirds);
        }
    }
}
