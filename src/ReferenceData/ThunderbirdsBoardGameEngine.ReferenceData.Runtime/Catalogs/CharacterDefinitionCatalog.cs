using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal class CharacterDefinitionCatalog : ICharacterDefinitionCatalog
    {
        private readonly FrozenDictionary<CharacterCode, ReferenceCharacterDefinition> _byCode;

        private readonly ImmutableArray<ReferenceCharacterDefinition> _characters;

        public CharacterDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _byCode = snapshot.CharacterDefinitions
                .ToDictionary(c => c.Code)
                .ToFrozenDictionary();

            _characters = snapshot.CharacterDefinitions.ToImmutableArray();
        }

        public ImmutableArray<ReferenceCharacterDefinition> GetAll()
        {
            return _characters;
        }

        public ReferenceCharacterDefinition GetByCode(CharacterCode code)
        {
            if (!_byCode.TryGetValue(code, out var character))
            {
                throw new KeyNotFoundException($"No character found with code '{code}'.");
            }

            return character;
        }
    }
}
